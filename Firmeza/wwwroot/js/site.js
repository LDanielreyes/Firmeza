// Enhanced JavaScript for Firmeza Application
// Toast Notification System
const Toast = {
    show: function (message, type = 'info', duration = 3000) {
        const toast = document.createElement('div');
        toast.className = `toast-notification toast-${type} fade-in`;
        toast.innerHTML = `
            <div class="toast-content">
                <span class="toast-icon">${this.getIcon(type)}</span>
                <span class="toast-message">${message}</span>
                <button class="toast-close" onclick="this.parentElement.parentElement.remove()">×</button>
            </div>
        `;

        document.body.appendChild(toast);

        setTimeout(() => {
            toast.classList.add('fade-out');
            setTimeout(() => toast.remove(), 300);
        }, duration);
    },

    getIcon: function (type) {
        const icons = {
            'success': '✓',
            'error': '✗',
            'warning': '⚠',
            'info': 'ℹ'
        };
        return icons[type] || icons['info'];
    }
};

// Add toast styles dynamically
const toastStyles = document.createElement('style');
toastStyles.textContent = `
    .toast-notification {
        position: fixed;
        top: 20px;
        right: 20px;
        background: white;
        padding: 1rem 1.5rem;
        border-radius: 0.75rem;
        box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
        z-index: 9999;
        min-width: 300px;
        max-width: 400px;
        transition: all 0.3s ease;
    }
    
    .toast-content {
        display: flex;
        align-items: center;
        gap: 0.75rem;
    }
    
    .toast-icon {
        font-size: 1.25rem;
        font-weight: bold;
    }
    
    .toast-message {
        flex: 1;
        font-weight: 500;
    }
    
    .toast-close {
        background: none;
        border: none;
        font-size: 1.5rem;
        cursor: pointer;
        color: #999;
        padding: 0;
        width: 24px;
        height: 24px;
        display: flex;
        align-items: center;
        justify-content: center;
        border-radius: 50%;
        transition: all 0.2s;
    }
    
    .toast-close:hover {
        background: #f0f0f0;
        color: #333;
    }
    
    .toast-success {
        border-left: 4px solid #10b981;
    }
    
    .toast-success .toast-icon {
        color: #10b981;
    }
    
    .toast-error {
        border-left: 4px solid #ef4444;
    }
    
    .toast-error .toast-icon {
        color: #ef4444;
    }
    
    .toast-warning {
        border-left: 4px solid #f59e0b;
    }
    
    .toast-warning .toast-icon {
        color: #f59e0b;
    }
    
    .toast-info {
        border-left: 4px solid #3b82f6;
    }
    
    .toast-info .toast-icon {
        color: #3b82f6;
    }
    
    .fade-out {
        opacity: 0;
        transform: translateX(100%);
    }
    
    @media (max-width: 768px) {
        .toast-notification {
            right: 10px;
            left: 10px;
            min-width: auto;
        }
    }
`;
document.head.appendChild(toastStyles);

// Smooth Scroll Enhancement
document.addEventListener('DOMContentLoaded', function () {
    // Add fade-in animation to main content
    const mainContent = document.querySelector('main');
    if (mainContent) {
        mainContent.classList.add('fade-in');
    }

    // Enhance all forms with loading states
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn && !submitBtn.classList.contains('no-loading')) {
                submitBtn.classList.add('loading');
                submitBtn.disabled = true;
                const originalText = submitBtn.textContent;
                submitBtn.innerHTML = '<span class="spinner"></span> Processing...';

                // Re-enable after 5 seconds as fallback
                setTimeout(() => {
                    submitBtn.disabled = false;
                    submitBtn.classList.remove('loading');
                    submitBtn.textContent = originalText;
                }, 5000);
            }
        });
    });

    // Add number input +/- buttons enhancement
    const numberInputs = document.querySelectorAll('input[type="number"]');
    numberInputs.forEach(input => {
        const wrapper = document.createElement('div');
        wrapper.className = 'number-input-wrapper';
        input.parentNode.insertBefore(wrapper, input);
        wrapper.appendChild(input);

        const decrementBtn = document.createElement('button');
        decrementBtn.type = 'button';
        decrementBtn.className = 'number-btn number-decrement';
        decrementBtn.textContent = '−';
        decrementBtn.onclick = function (e) {
            e.preventDefault();
            const currentValue = parseInt(input.value) || 0;
            const min = parseInt(input.min) || 0;
            if (currentValue > min) {
                input.value = currentValue - 1;
                input.dispatchEvent(new Event('change'));
            }
        };

        const incrementBtn = document.createElement('button');
        incrementBtn.type = 'button';
        incrementBtn.className = 'number-btn number-increment';
        incrementBtn.textContent = '+';
        incrementBtn.onclick = function (e) {
            e.preventDefault();
            const currentValue = parseInt(input.value) || 0;
            const max = parseInt(input.max) || 999;
            if (currentValue < max) {
                input.value = currentValue + 1;
                input.dispatchEvent(new Event('change'));
            }
        };

        wrapper.insertBefore(decrementBtn, input);
        wrapper.appendChild(incrementBtn);
    });

    // Add confirmation to destructive actions
    const dangerButtons = document.querySelectorAll('.btn-danger, .btn-outline-danger');
    dangerButtons.forEach(btn => {
        if (!btn.classList.contains('no-confirm')) {
            btn.addEventListener('click', function (e) {
                const confirmMsg = this.dataset.confirm || 'Are you sure you want to proceed?';
                if (!confirm(confirmMsg)) {
                    e.preventDefault();
                    return false;
                }
            });
        }
    });

    // Animate cards on scroll (opt-in with .animate-on-scroll class)
    const cards = document.querySelectorAll('.card.animate-on-scroll');
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const cardObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry, index) => {
            if (entry.isIntersecting) {
                setTimeout(() => {
                    entry.target.style.opacity = '1';
                    entry.target.classList.add('fade-in');
                }, index * 50);
                cardObserver.unobserve(entry.target);
            }
        });
    }, observerOptions);

    cards.forEach(card => {
        card.style.opacity = '0';
        cardObserver.observe(card);
    });

    // Add ripple effect to buttons
    document.querySelectorAll('.btn').forEach(button => {
        button.addEventListener('click', function (e) {
            const ripple = document.createElement('span');
            ripple.className = 'ripple';
            this.appendChild(ripple);

            const rect = this.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;

            ripple.style.width = ripple.style.height = size + 'px';
            ripple.style.left = x + 'px';
            ripple.style.top = y + 'px';

            setTimeout(() => ripple.remove(), 600);
        });
    });
});

// Add spinner and ripple styles
const additionalStyles = document.createElement('style');
additionalStyles.textContent = `
    .spinner {
        display: inline-block;
        width: 14px;
        height: 14px;
        border: 2px solid rgba(255, 255, 255, 0.3);
        border-top-color: white;
        border-radius: 50%;
        animation: spin 0.6s linear infinite;
    }
    
    @keyframes spin {
        to { transform: rotate(360deg); }
    }
    
    .number-input-wrapper {
        display: inline-flex;
        align-items: center;
        gap: 0.25rem;
        background: white;
        border-radius: 0.5rem;
        padding: 0.25rem;
        border: 2px solid #e2e8f0;
    }
    
    .number-input-wrapper input[type="number"] {
        border: none !important;
        text-align: center;
        width: 60px;
        padding: 0.375rem 0.5rem !important;
        box-shadow: none !important;
    }
    
    .number-btn {
        width: 28px;
        height: 28px;
        border: none;
        background: linear-gradient(135deg, hsl(210, 100%, 50%), hsl(210, 100%, 60%));
        color: white;
        border-radius: 0.375rem;
        cursor: pointer;
        font-weight: bold;
        font-size: 1rem;
        display: flex;
        align-items: center;
        justify-content: center;
        transition: all 0.2s;
    }
    
    .number-btn:hover {
        transform: scale(1.1);
        box-shadow: 0 2px 8px rgba(0, 128, 255, 0.4);
    }
    
    .number-btn:active {
        transform: scale(0.95);
    }
    
    .ripple {
        position: absolute;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.6);
        transform: scale(0);
        animation: ripple-animation 0.6s ease-out;
        pointer-events: none;
    }
    
    @keyframes ripple-animation {
        to {
            transform: scale(2);
            opacity: 0;
        }
    }
`;
document.head.appendChild(additionalStyles);

// Expose Toast globally
window.Toast = Toast;
