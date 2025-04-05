// Hiệu ứng gõ chữ trên trang chủ
class TypeWriter {
    constructor(txtElement, words, wait = 3000) {
        this.txtElement = txtElement;
        this.words = words;
        this.txt = '';
        this.wordIndex = 0;
        this.wait = parseInt(wait, 10);
        this.type();
        this.isDeleting = false;
    }

    type() {
        // Chỉ số từ hiện tại
        const current = this.wordIndex % this.words.length;
        // Lấy toàn bộ văn bản của từ hiện tại
        const fullTxt = this.words[current];

        // Kiểm tra nếu đang xóa
        if (this.isDeleting) {
            // Xóa
            this.txt = fullTxt.substring(0, this.txt.length - 1);
        } else {
            // Thêm
            this.txt = fullTxt.substring(0, this.txt.length + 1);
        }

        // Cập nhật phần tử
        this.txtElement.textContent = this.txt;

        // Tốc độ gõ
        let typeSpeed = 120;

        if (this.isDeleting) {
            typeSpeed /= 2;
        }

        // Nếu từ đã hoàn thành
        if (!this.isDeleting && this.txt === fullTxt) {
            // Tạm dừng ở cuối
            typeSpeed = this.wait;
            // Đặt delete thành true
            this.isDeleting = true;
        } else if (this.isDeleting && this.txt === '') {
            this.isDeleting = false;
            // Chuyển sang từ tiếp theo
            this.wordIndex++;
            // Tạm dừng trước khi gõ
            typeSpeed = 500;
        }

        setTimeout(() => this.type(), typeSpeed);
    }
}

// Khởi tạo hiệu ứng rotation khi trang tải
function initRotationEffect() {
    const heroLogo = document.querySelector('.ai-hero-logo');
    if (heroLogo) {
        heroLogo.addEventListener('mouseover', () => {
            heroLogo.style.transform = 'rotate(10deg)';
        });
        
        heroLogo.addEventListener('mouseout', () => {
            heroLogo.style.transform = 'rotate(0deg)';
        });
    }
}

// Khởi tạo hiệu ứng hover cho các thẻ dịch vụ
function initServiceCardEffects() {
    const serviceCards = document.querySelectorAll('.ai-service-card');
    
    serviceCards.forEach(card => {
        card.addEventListener('mouseenter', () => {
            const icon = card.querySelector('.ai-service-icon');
            if (icon) {
                icon.style.transform = 'scale(1.1)';
                icon.style.transition = 'transform 0.3s ease';
            }
        });
        
        card.addEventListener('mouseleave', () => {
            const icon = card.querySelector('.ai-service-icon');
            if (icon) {
                icon.style.transform = 'scale(1)';
            }
        });
    });
}

// Tạo hiệu ứng particle động
function createParticles() {
    const heroSection = document.querySelector('.ai-hero');
    if (!heroSection) return;
    
    for (let i = 0; i < 30; i++) {
        const particle = document.createElement('div');
        particle.classList.add('dynamic-particle');
        
        // Random kích thước
        const size = Math.random() * 8 + 3;
        particle.style.width = `${size}px`;
        particle.style.height = `${size}px`;
        
        // Random vị trí
        particle.style.left = `${Math.random() * 100}%`;
        particle.style.top = `${Math.random() * 100}%`;
        
        // Random độ trễ
        particle.style.animationDelay = `${Math.random() * 5}s`;
        
        // Random màu sắc
        const colors = ['rgba(76, 201, 240, 0.6)', 'rgba(247, 37, 133, 0.6)', 'rgba(67, 97, 238, 0.6)'];
        particle.style.backgroundColor = colors[Math.floor(Math.random() * colors.length)];
        
        heroSection.appendChild(particle);
    }
}

// Scroll effect
function initScrollEffects() {
    window.addEventListener('scroll', () => {
        const sections = document.querySelectorAll('.ai-section, .ai-about-section');
        const scrollPosition = window.scrollY + window.innerHeight * 0.8;
        
        sections.forEach(section => {
            const sectionTop = section.offsetTop;
            
            if (scrollPosition > sectionTop) {
                section.classList.add('animate__animated', 'animate__fadeIn');
                section.style.opacity = '1';
            }
        });
    });
}

// Chuẩn bị elements cho typing effect
function prepareTypingText() {
    const typingElement = document.querySelector('.ai-typing-text');
    if (!typingElement) return;
    
    const words = typingElement.querySelectorAll('span');
    const wordsArray = [];
    
    words.forEach(word => {
        wordsArray.push(word.textContent);
    });
    
    // Hiển thị phần tử đầu tiên
    if (words[0]) {
        words[0].style.display = 'inline-block';
    }
    
    // Bắt đầu typing effect
    new TypeWriter(words[0], wordsArray, 3000);
}

// Khởi tạo tất cả hiệu ứng khi trang tải xong
document.addEventListener('DOMContentLoaded', () => {
    initRotationEffect();
    initServiceCardEffects();
    createParticles();
    initScrollEffects();
    prepareTypingText();
    
    // Thêm các lớp animate cho hero content
    const heroContent = document.querySelector('.ai-hero-content');
    if (heroContent) {
        heroContent.style.opacity = '0';
        setTimeout(() => {
            heroContent.style.opacity = '1';
        }, 300);
    }
    
    // Thêm hiệu ứng cho các phần tử dịch vụ
    const serviceCards = document.querySelectorAll('.ai-service-card');
    serviceCards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
        
        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 300 + (index * 150));
    });
});

// Thêm hiệu ứng khi chuyển trang
window.addEventListener('beforeunload', () => {
    const mainContent = document.querySelector('main');
    if (mainContent) {
        mainContent.style.opacity = '0';
        mainContent.style.transform = 'translateY(30px)';
    }
}); 