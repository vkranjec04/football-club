// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(function () {
    // ===== DELETED ROWS FADE-OUT ANIMATION =====
    function initDeletedRowAnimations() {
        const deletedRows = document.querySelectorAll('tr[style*="opacity"]');
        deletedRows.forEach(function (row) {
            // Add a fade-in animation on page load for deleted rows
            row.style.animation = 'fadeInDeleted 0.6s ease-in-out';
        });
    }

    // ===== FORM VALIDATION ERROR ANIMATIONS =====
    function initValidationErrorAnimations() {
        const validationMessages = document.querySelectorAll('[data-valmsg-for]');
        validationMessages.forEach(function (msg) {
            if (msg.textContent && msg.textContent.trim()) {
                // Slide down error message with smooth animation
                msg.style.maxHeight = '0';
                msg.style.overflow = 'hidden';
                msg.style.transition = 'maxHeight 0.35s ease-out, opacity 0.35s ease-out';
                msg.style.opacity = '0';
                msg.style.color = '#fca5a5';
                msg.style.fontSize = '13px';
                msg.style.marginTop = '4px';

                // Trigger animation
                setTimeout(function () {
                    msg.style.maxHeight = '100px';
                    msg.style.opacity = '1';
                }, 10);
            }
        });

        // Listen for real-time validation on form inputs
        const formInputs = document.querySelectorAll('.form-control');
        formInputs.forEach(function (input) {
            input.addEventListener('change', function () {
                const errorMsg = this.parentElement.querySelector('[data-valmsg-for]');
                if (errorMsg) {
                    if (errorMsg.textContent && errorMsg.textContent.trim()) {
                        errorMsg.style.maxHeight = '100px';
                        errorMsg.style.opacity = '1';
                    } else {
                        errorMsg.style.maxHeight = '0';
                        errorMsg.style.opacity = '0';
                    }
                }
            });
        });
    }

    // ===== TABLE ROW HOVER ANIMATIONS =====
    function initTableRowHoverAnimations() {
        const tableRows = document.querySelectorAll('.fc-table tbody tr');
        tableRows.forEach(function (row) {
            row.addEventListener('mouseenter', function () {
                if (!this.style.opacity || this.style.opacity === '1') {
                    // Only animate non-deleted rows
                    this.style.transition = 'all 0.2s ease-out';
                    this.style.backgroundColor = 'rgba(59, 130, 246, 0.08)';
                    this.style.transform = 'scale(1.01)';
                    this.style.boxShadow = '0 4px 12px rgba(59, 130, 246, 0.12)';
                }
            });

            row.addEventListener('mouseleave', function () {
                this.style.transition = 'all 0.2s ease-out';
                this.style.backgroundColor = '#1e293b';
                this.style.transform = 'scale(1)';
                this.style.boxShadow = 'none';
            });
        });
    }

    // ===== BUTTON CLICK FADE ANIMATION =====
    function initButtonAnimations() {
        const buttons = document.querySelectorAll('.btn, .btn-back, .view-link, button[type="submit"]');
        buttons.forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                // Create a ripple effect on click
                if (this.tagName === 'BUTTON' || this.className.includes('btn')) {
                    const ripple = document.createElement('span');
                    ripple.style.position = 'absolute';
                    ripple.style.width = '20px';
                    ripple.style.height = '20px';
                    ripple.style.background = 'rgba(255,255,255,0.5)';
                    ripple.style.borderRadius = '50%';
                    ripple.style.pointerEvents = 'none';
                    ripple.style.animation = 'ripple 0.6s ease-out';

                    // Position ripple at click point
                    const rect = this.getBoundingClientRect();
                    ripple.style.left = (e.clientX - rect.left - 10) + 'px';
                    ripple.style.top = (e.clientY - rect.top - 10) + 'px';

                    this.style.position = 'relative';
                    this.style.overflow = 'hidden';
                    this.appendChild(ripple);

                    setTimeout(function () {
                        ripple.remove();
                    }, 600);
                }
            });
        });
    }

    // ===== PAGE LOAD INITIALIZATION =====
    document.addEventListener('DOMContentLoaded', function () {
        initDeletedRowAnimations();
        initValidationErrorAnimations();
        initTableRowHoverAnimations();
        initButtonAnimations();
    });

    // Re-initialize animations when AJAX updates table rows
    window.reinitializeAnimations = function () {
        initTableRowHoverAnimations();
    };
})();
