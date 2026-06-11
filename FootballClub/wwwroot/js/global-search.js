// =============================================
//  GLOBAL SEARCH (topbar)
//  Hybrid navigator: a static page/menu index (instant, client-side) plus
//  live record search via the existing /api/{players,staff,training}/search
//  endpoints (all return [{ id, text, subtitle, url }]).
//  Loaded globally by _Layout AFTER auth.js so admin-only pages can be gated
//  on Auth.getRole(). Markup lives in Views/Shared/_GlobalSearch.cshtml.
// =============================================
(function () {
    "use strict";

    // ----- Static index of pages / menu items -----
    // Keep in sync with the sidebar nav in _Layout.cshtml and sitemap.md.
    // keywords are stored diacritic-free (see normalize); user input is
    // normalized the same way so "igraci" and the accented form both match.
    var PAGES = [
        { title: "Dashboard", url: "/", icon: "🏠", keywords: ["pocetna", "naslovnica", "home", "pregled", "dashboard"] },
        { title: "Players", url: "/team-roster", icon: "👤", keywords: ["igraci", "roster", "momcad", "squad", "players"] },
        { title: "Schedules", url: "/Player/Schedule", icon: "📋", keywords: ["raspored", "obveze", "kalendar", "schedule", "schedules"] },
        { title: "Training", url: "/Training", icon: "🧪", keywords: ["trening", "treninzi", "vjezbe", "training"] },
        { title: "Matches", url: "/Match", icon: "⚽", keywords: ["utakmice", "rezultati", "fixtures", "matches"] },
        { title: "Staff", url: "/staff-members", icon: "👨‍🏫", keywords: ["osoblje", "treneri", "stozer", "coaches", "staff"] },
        { title: "Medical Center", url: "/Medical", icon: "🏥", keywords: ["medicina", "ozljede", "injured", "medical", "ambulanta"] },
        { title: "Tactics Board", url: "/Tactics", icon: "📋", keywords: ["taktika", "formacija", "postava", "tactics"] },
        { title: "League standings", url: "/league-standings", icon: "🏆", keywords: ["liga", "tablica", "poredak", "standings", "league"] },
        { title: "Stadiums", url: "/stadiums/list", icon: "🏟️", keywords: ["stadion", "stadioni", "stadiums"] },
        { title: "New player", url: "/Player/Create", icon: "➕", keywords: ["novi igrac", "dodaj igraca", "create player", "new player"] },
        { title: "New training", url: "/Training/Create", icon: "➕", keywords: ["novi trening", "dodaj trening", "create training", "new training"] },
        { title: "New staff", url: "/Staff/Create", icon: "➕", keywords: ["novo osoblje", "dodaj clana", "create staff", "new staff"] },
        { title: "Activity Log", url: "/ActivityLog", icon: "📜", keywords: ["log", "aktivnost", "audit", "revizija", "activity"], adminOnly: true }
    ];

    // ----- Live record sources (existing anonymous GET endpoints) -----
    var RECORD_SOURCES = [
        { label: "Igrači", icon: "👤", endpoint: "/api/players/search" },
        { label: "Osoblje", icon: "👨‍🏫", endpoint: "/api/staff/search" },
        { label: "Treninzi", icon: "🧪", endpoint: "/api/training/search" }
    ];

    var MIN_RECORD_CHARS = 2;   // record endpoints ignore shorter terms
    var DEBOUNCE_MS = 200;
    var GROUP_LIMIT = 6;

    // Combining diacritical marks (U+0300..U+036F) and d-with-stroke (U+0111),
    // built from char codes so the matching logic stays pure ASCII and cannot
    // be broken by a file-encoding mishap.
    var RE_DIACRITICS = new RegExp("[" + String.fromCharCode(0x300) + "-" + String.fromCharCode(0x36f) + "]", "g");
    var RE_DSTROKE = new RegExp(String.fromCharCode(0x111), "g");

    var root, input, panel;
    var debounceTimer = null;
    var currentController = null;
    var requestSeq = 0;          // guards against stale async renders
    var flatItems = [];          // item data, in render order (for keyboard nav)
    var itemEls = [];            // parallel DOM nodes
    var activeIndex = -1;

    // Strip case + diacritics so "igraci" matches the accented spelling. NFD
    // splits most accents into combining marks we then drop; the stroke in
    // d-with-stroke does not decompose, so it is replaced explicitly first.
    function normalize(value) {
        return (value || "")
            .toString()
            .toLowerCase()
            .replace(RE_DSTROKE, "d")
            .normalize("NFD")
            .replace(RE_DIACRITICS, "");
    }

    function isAdmin() {
        return typeof Auth !== "undefined" && !!Auth.getToken() && Auth.getRole() === "Admin";
    }

    function searchPages(query) {
        var nq = normalize(query);
        var admin = isAdmin();
        return PAGES.filter(function (page) {
            if (page.adminOnly && !admin) {
                return false;
            }
            if (normalize(page.title).indexOf(nq) !== -1) {
                return true;
            }
            return page.keywords.some(function (kw) {
                return normalize(kw).indexOf(nq) !== -1;
            });
        }).slice(0, GROUP_LIMIT);
    }

    function fetchSource(source, query, signal) {
        return fetch(source.endpoint + "?term=" + encodeURIComponent(query), {
            headers: { "Accept": "application/json" },
            signal: signal
        })
            .then(function (response) { return response.ok ? response.json() : []; })
            .then(function (items) {
                return { label: source.label, icon: source.icon, items: (items || []).slice(0, GROUP_LIMIT) };
            })
            .catch(function () {
                return { label: source.label, icon: source.icon, items: [] };
            });
    }

    function fetchAllRecords(query) {
        if (currentController) {
            currentController.abort();
        }
        currentController = new AbortController();
        var signal = currentController.signal;
        var seq = ++requestSeq;

        Promise.all(RECORD_SOURCES.map(function (src) {
            return fetchSource(src, query, signal);
        })).then(function (groups) {
            if (seq !== requestSeq) {
                return; // a newer search superseded this one
            }
            render(query, searchPages(query), groups, false);
        });
    }

    function invalidatePending() {
        requestSeq++;
        if (currentController) {
            currentController.abort();
            currentController = null;
        }
    }

    // ----- Rendering -----
    function makeItem(item) {
        var idx = flatItems.length;
        var el = document.createElement("div");
        el.className = "global-search__item";
        el.id = "gs-opt-" + idx;
        el.setAttribute("role", "option");

        var icon = document.createElement("span");
        icon.className = "global-search__item-icon";
        icon.setAttribute("aria-hidden", "true");
        icon.textContent = item.icon || "•";

        var body = document.createElement("div");
        body.className = "global-search__item-body";

        var title = document.createElement("div");
        title.className = "global-search__item-title";
        title.textContent = item.text || "";
        body.appendChild(title);

        if (item.subtitle) {
            var sub = document.createElement("div");
            sub.className = "global-search__item-sub";
            sub.textContent = item.subtitle;
            body.appendChild(sub);
        }

        el.appendChild(icon);
        el.appendChild(body);

        // mousedown (not click) fires before the input blur, so the panel
        // is still open when we navigate.
        el.addEventListener("mousedown", function (event) {
            event.preventDefault();
            go(item.url);
        });
        el.addEventListener("mouseenter", function () { setActive(idx); });

        flatItems.push(item);
        itemEls.push(el);
        return el;
    }

    function appendGroup(label, items) {
        var header = document.createElement("div");
        header.className = "global-search__group-label";
        header.textContent = label;
        panel.appendChild(header);
        items.forEach(function (item) { panel.appendChild(makeItem(item)); });
    }

    function appendNote(text) {
        var note = document.createElement("div");
        note.className = "global-search__note";
        note.textContent = text;
        panel.appendChild(note);
    }

    function render(query, pages, recordGroups, loading) {
        panel.innerHTML = "";
        flatItems = [];
        itemEls = [];
        activeIndex = -1;

        var hasResults = false;

        if (pages.length) {
            appendGroup("Stranice", pages.map(function (page) {
                return { text: page.title, subtitle: page.url, url: page.url, icon: page.icon };
            }));
            hasResults = true;
        }

        (recordGroups || []).forEach(function (group) {
            if (group.items && group.items.length) {
                appendGroup(group.label, group.items.map(function (rec) {
                    return { text: rec.text, subtitle: rec.subtitle, url: rec.url, icon: group.icon };
                }));
                hasResults = true;
            }
        });

        if (loading) {
            appendNote("Pretrazivanje zapisa...");
        } else if (!hasResults) {
            appendNote("Nema rezultata za \"" + query + "\".");
        }

        openPanel();
        if (flatItems.length) {
            setActive(0);
        }
    }

    // ----- Panel / selection state -----
    function openPanel() {
        panel.hidden = false;
        input.setAttribute("aria-expanded", "true");
    }

    function closePanel() {
        panel.hidden = true;
        input.setAttribute("aria-expanded", "false");
        input.removeAttribute("aria-activedescendant");
        activeIndex = -1;
    }

    function setActive(index) {
        activeIndex = index;
        itemEls.forEach(function (el, i) {
            if (i === index) {
                el.classList.add("is-active");
                input.setAttribute("aria-activedescendant", el.id);
                el.scrollIntoView({ block: "nearest" });
            } else {
                el.classList.remove("is-active");
            }
        });
    }

    function move(delta) {
        if (!flatItems.length) {
            return;
        }
        var next = activeIndex < 0
            ? (delta > 0 ? 0 : flatItems.length - 1)
            : (activeIndex + delta + flatItems.length) % flatItems.length;
        setActive(next);
    }

    function go(url) {
        if (url) {
            window.location.href = url;
        }
    }

    // ----- Search entry point -----
    function runSearch(rawQuery) {
        var query = (rawQuery || "").trim();
        clearTimeout(debounceTimer);

        if (!query) {
            invalidatePending();
            closePanel();
            return;
        }

        var pages = searchPages(query);
        var wantRecords = query.length >= MIN_RECORD_CHARS;

        // Pages are instant; show them now and flag record loading.
        render(query, pages, [], wantRecords);

        if (wantRecords) {
            debounceTimer = window.setTimeout(function () { fetchAllRecords(query); }, DEBOUNCE_MS);
        } else {
            invalidatePending();
        }
    }

    function init() {
        root = document.getElementById("globalSearch");
        if (!root) {
            return;
        }
        input = document.getElementById("globalSearchInput");
        panel = document.getElementById("globalSearchPanel");

        input.addEventListener("input", function () { runSearch(input.value); });

        input.addEventListener("focus", function () {
            if (input.value.trim()) {
                runSearch(input.value);
            }
        });

        input.addEventListener("keydown", function (event) {
            switch (event.key) {
                case "ArrowDown":
                    event.preventDefault();
                    move(1);
                    break;
                case "ArrowUp":
                    event.preventDefault();
                    move(-1);
                    break;
                case "Enter":
                    if (activeIndex >= 0 && flatItems[activeIndex]) {
                        event.preventDefault();
                        go(flatItems[activeIndex].url);
                    }
                    break;
                case "Escape":
                    closePanel();
                    input.blur();
                    break;
            }
        });

        // Ctrl+K / Cmd+K focuses the search from anywhere.
        document.addEventListener("keydown", function (event) {
            if ((event.ctrlKey || event.metaKey) && (event.key === "k" || event.key === "K")) {
                event.preventDefault();
                input.focus();
                input.select();
            }
        });

        // Click outside closes the panel.
        document.addEventListener("click", function (event) {
            if (!root.contains(event.target)) {
                closePanel();
            }
        });
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", init);
    } else {
        init();
    }
})();
