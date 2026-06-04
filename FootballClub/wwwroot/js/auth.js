/**
 * Shared auth helpers.
 * Stores the JWT issued by /api/auth/login and /api/auth/register in localStorage
 * and exposes small utilities used by the auth pages, the layout widget and the
 * attachment upload component.
 */
const Auth = (() => {
    const TOKEN_KEY = 'fc_token';
    const USERNAME_KEY = 'fc_username';
    const ROLE_KEY = 'fc_role';

    function storeSession(data) {
        if (!data || !data.token) return;
        localStorage.setItem(TOKEN_KEY, data.token);
        localStorage.setItem(USERNAME_KEY, data.username || '');
        localStorage.setItem(ROLE_KEY, data.role || '');
    }

    function clearSession() {
        localStorage.removeItem(TOKEN_KEY);
        localStorage.removeItem(USERNAME_KEY);
        localStorage.removeItem(ROLE_KEY);
    }

    function getToken() {
        return localStorage.getItem(TOKEN_KEY);
    }

    function getUsername() {
        return localStorage.getItem(USERNAME_KEY);
    }

    function getRole() {
        return localStorage.getItem(ROLE_KEY);
    }

    /** Returns headers with an Authorization: Bearer entry when a token is present. */
    function authHeaders(headers) {
        const result = headers || {};
        const token = getToken();
        if (token) {
            result['Authorization'] = 'Bearer ' + token;
        }
        return result;
    }

    /** Extracts a readable message from an error response (ModelState or custom errors). */
    async function parseErrors(response) {
        try {
            const body = await response.json();
            const messages = [];
            if (body) {
                if (Array.isArray(body.errors)) {
                    messages.push(...body.errors);
                } else if (body.errors && typeof body.errors === 'object') {
                    Object.values(body.errors).forEach(value => {
                        if (Array.isArray(value)) messages.push(...value);
                        else messages.push(value);
                    });
                } else if (typeof body === 'string') {
                    messages.push(body);
                } else if (body.title) {
                    messages.push(body.title);
                }
            }
            return messages.length ? messages.join(' ') : 'Zahtjev nije uspio.';
        } catch (error) {
            return 'Zahtjev nije uspio.';
        }
    }

    function showAlert(elementId, message, type) {
        const alert = document.getElementById(elementId);
        if (!alert) return;
        alert.textContent = message;
        alert.className = 'alert alert-' + (type || 'info');
        alert.style.display = 'block';
        alert.style.padding = '10px 12px';
        alert.style.borderRadius = '6px';
        if (type === 'danger') {
            alert.style.background = '#fee2e2';
            alert.style.color = '#991b1b';
        } else if (type === 'success') {
            alert.style.background = '#dcfce7';
            alert.style.color = '#166534';
        } else {
            alert.style.background = '#e0f2fe';
            alert.style.color = '#075985';
        }
    }

    return {
        storeSession: storeSession,
        clearSession: clearSession,
        getToken: getToken,
        getUsername: getUsername,
        getRole: getRole,
        authHeaders: authHeaders,
        parseErrors: parseErrors,
        showAlert: showAlert
    };
})();
