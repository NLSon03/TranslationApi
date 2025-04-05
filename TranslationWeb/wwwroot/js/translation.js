window.initNeonHoverEffects = function() {
    const elements = document.querySelectorAll('.ai-neon-hover');
    
    elements.forEach(element => {
        // Simple visual feedback on hover
        element.addEventListener('mouseenter', () => {
            element.style.transition = 'box-shadow 0.3s ease, transform 0.3s ease';
            // Example: Slightly enhance existing shadow or add a glow
            // This needs coordination with the CSS definitions
        });
        
        element.addEventListener('mouseleave', () => {
             element.style.transition = 'box-shadow 0.3s ease, transform 0.3s ease';
             // Reset styles if needed, or rely on CSS :hover pseudo-class
        });
    });
};

window.initGlitchBorders = function(containerSelector = '.ai-glitch-border') {
    const elements = document.querySelectorAll(containerSelector);
    
    elements.forEach(element => {
        // Ensure the container can hold the absolutely positioned pseudo-element
        if (window.getComputedStyle(element).position === 'static') {
            element.style.position = 'relative';
        }
        // Add a class to trigger CSS-based glitch effect instead of JS interval
        element.classList.add('css-glitch-effect'); 
    });
}; 