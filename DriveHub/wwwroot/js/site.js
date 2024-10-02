// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.addEventListener('DOMContentLoaded', function () {
    const heroHeading = document.querySelector('.hero-heading');

    // A check if heroHeading exists so it doesn't error on other pages
    if (heroHeading) {
        setTimeout(() => {
            heroHeading.classList.add('scrolled');
        }, 500);
    }

    const fadeInOnScroll = () => {
        const scrollPosition = window.scrollY;
        const opacity = Math.min(1, scrollPosition / (windowHeight / 2));

        heroHeading.style.opacity = opacity;

        if (scrollPosition <= windowHeight) {
            heroHeading.style.transition = 'opacity 0.5s ease-out';
        }
    };

    window.addEventListener('scroll', fadeInOnScroll)
});