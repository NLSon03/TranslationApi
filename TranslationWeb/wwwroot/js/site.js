﻿// Đăng ký tất cả các hàm vào đối tượng window ngay từ đầu
window.scrollToBottom = function(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

window.formatTimestamp = function(timestamp) {
    const date = new Date(timestamp);
    return date.toLocaleTimeString();
};

window.showToast = function(message, type = 'info', duration = 3000) {
    // Tạo phần tử toast
    const toast = document.createElement('div');
    toast.className = `ai-toast ai-toast-${type}`;
    
    // Thêm biểu tượng dựa vào loại thông báo
    let icon = 'info-circle';
    if (type === 'success') icon = 'check-circle';
    if (type === 'error') icon = 'exclamation-circle';
    if (type === 'warning') icon = 'exclamation-triangle';
    
    // Nội dung toast
    toast.innerHTML = `
        <div class="ai-toast-icon">
            <i class="fas fa-${icon}"></i>
        </div>
        <div class="ai-toast-content">${message}</div>
        <button class="ai-toast-close"><i class="fas fa-times"></i></button>
    `;
    
    // Container cho toasts
    let container = document.querySelector('.ai-toast-container');
    if (!container) {
        container = document.createElement('div');
        container.className = 'ai-toast-container';
        document.body.appendChild(container);
    }
    
    // Thêm toast vào container
    container.appendChild(toast);
    
    // Animation hiện ra
    setTimeout(() => {
        toast.classList.add('ai-toast-visible');
    }, 10);
    
    // Sự kiện đóng toast
    const closeBtn = toast.querySelector('.ai-toast-close');
    closeBtn.addEventListener('click', () => {
        toast.classList.remove('ai-toast-visible');
        setTimeout(() => {
            container.removeChild(toast);
        }, 300);
    });
    
    // Tự động đóng sau thời gian duration
    setTimeout(() => {
        if (document.body.contains(toast)) {
            toast.classList.remove('ai-toast-visible');
            setTimeout(() => {
                if (container.contains(toast)) {
                    container.removeChild(toast);
                }
            }, 300);
        }
    }, duration);
};

window.initTypeWriter = function() {
    const elements = document.querySelectorAll('.ai-type-writer');
    
    elements.forEach(element => {
        const text = element.textContent;
        const speed = parseInt(element.getAttribute('data-speed') || '50', 10);
        
        // Xóa nội dung hiện tại
        element.textContent = '';
        
        // Thêm con trỏ
        const cursor = document.createElement('span');
        cursor.className = 'ai-type-cursor';
        cursor.textContent = '|';
        element.appendChild(cursor);
        
        // Hiệu ứng gõ chữ
        let i = 0;
        const typeInterval = setInterval(() => {
            if (i < text.length) {
                const span = document.createElement('span');
                span.textContent = text.charAt(i);
                element.insertBefore(span, cursor);
                i++;
            } else {
                clearInterval(typeInterval);
                
                // Animation nhấp nháy con trỏ
                cursor.classList.add('ai-cursor-blink');
            }
        }, speed);
    });
};

window.createParticlesBackground = function(containerId, count = 50) {
    const container = document.getElementById(containerId);
    if (!container) return;
    
    // Check if particles already exist
    if (container.querySelector('.ai-particle')) {
        return;
    }

    container.classList.add('ai-particles-container');
    
    for (let i = 0; i < count; i++) {
        const particle = document.createElement('span');
        particle.className = 'ai-particle';
        
        // Ngẫu nhiên các thuộc tính
        const size = Math.random() * 3 + 1; // Smaller particles
        particle.style.width = `${size}px`;
        particle.style.height = `${size}px`;
        particle.style.left = `${Math.random() * 100}%`;
        particle.style.top = `${Math.random() * 100}%`;
        particle.style.animationDuration = `${Math.random() * 25 + 15}s`; // Slower animation
        particle.style.animationDelay = `${Math.random() * 10}s`; // Longer delay variance
        
        // Ngẫu nhiên màu sắc
        const colors = ['#4cc9f0', '#4361ee', '#7209b7', '#f72585']; // Adjusted colors slightly
        particle.style.backgroundColor = colors[Math.floor(Math.random() * colors.length)];
        particle.style.opacity = `${Math.random() * 0.5 + 0.2}`; // Slightly more transparent

        container.appendChild(particle);
    }
};

window.animatePageTransition = function() {
    const mainContent = document.querySelector('.main-content');
    if (mainContent) {
        mainContent.style.opacity = '0';
        mainContent.style.transform = 'translateY(20px)';
        
        setTimeout(() => {
            mainContent.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
            mainContent.style.opacity = '1';
            mainContent.style.transform = 'translateY(0)';
        }, 100);
    }
};

window.createDynamicParticles = function() {
    const particlesContainer = document.querySelector('.ai-particles');
    if (!particlesContainer) return;
    
    const colors = ['rgba(76, 201, 240, 0.6)', 'rgba(247, 37, 133, 0.6)'];
    
    for (let i = 0; i < 30; i++) {
        const particle = document.createElement('div');
        particle.className = 'dynamic-particle';
        
        // Random properties
        const size = Math.random() * 8 + 2;
        const colorIndex = Math.floor(Math.random() * colors.length);
        
        particle.style.width = `${size}px`;
        particle.style.height = `${size}px`;
        particle.style.backgroundColor = colors[colorIndex];
        particle.style.left = `${Math.random() * 100}%`;
        particle.style.top = `${Math.random() * 100}%`;
        particle.style.animationDuration = `${Math.random() * 20 + 10}s`;
        particle.style.animationDelay = `${Math.random() * 5}s`;
        
        particlesContainer.appendChild(particle);
    }
};

// Hiệu ứng hover cho brand và các sự kiện DOM
document.addEventListener('DOMContentLoaded', function () {
    // Tạo hiệu ứng cho brand khi hover
    const brandIcon = document.querySelector('.brand-icon');
    if (brandIcon) {
        brandIcon.addEventListener('mouseover', function () {
            this.style.transform = 'rotate(25deg)';
            setTimeout(() => {
                this.style.transform = 'rotate(0deg)';
            }, 300);
        });
    }

    // Hiệu ứng tự động cho brand khi load trang
    setTimeout(function() {
        if (brandIcon) {
            brandIcon.style.transition = 'transform 0.5s ease';
            brandIcon.style.transform = 'rotate(25deg)';
            setTimeout(() => {
                brandIcon.style.transform = 'rotate(0deg)';
            }, 500);
        }
    }, 1000);

    // Thêm hiệu ứng đặc biệt cho header
    const navbarBrand = document.querySelector('.navbar-brand');
    if (navbarBrand) {
        window.addEventListener('scroll', function() {
            if (window.scrollY > 50) {
                document.querySelector('.ai-navbar').classList.add('scrolled');
            } else {
                document.querySelector('.ai-navbar').classList.remove('scrolled');
            }
        });
    }

    // Hiệu ứng cho các thẻ tech-badge trong footer
    const techBadges = document.querySelectorAll('.tech-badge');
    if (techBadges.length > 0) {
        techBadges.forEach(badge => {
            badge.addEventListener('mouseover', function() {
                this.style.transform = 'translateY(-5px)';
            });
            
            badge.addEventListener('mouseout', function() {
                this.style.transform = 'translateY(0)';
            });
        });
    }

    // Khởi tạo hiệu ứng khi DOM đã tải
    window.animatePageTransition();
    window.createDynamicParticles();
    window.initTypeWriter();
    
    // Tạo particles cho các container cụ thể
    const particleContainers = document.querySelectorAll('[data-particles]');
    particleContainers.forEach(container => {
        const count = parseInt(container.getAttribute('data-particles-count') || '50', 10);
        window.createParticlesBackground(container.id, count);
    });
});