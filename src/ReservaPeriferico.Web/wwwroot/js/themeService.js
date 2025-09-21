window.themeService = {
    getTheme: function () {
        return localStorage.getItem('theme') || 'light';
    },
    
    setTheme: function (theme) {
        localStorage.setItem('theme', theme);
        this.applyTheme(theme);
    },
    
    isDarkMode: function () {
        return this.getTheme() === 'dark';
    },
    
    applyTheme: function (theme) {
        const isDark = theme === 'dark';
        document.documentElement.setAttribute('data-theme', theme);
        document.body.classList.toggle('mud-theme-dark', isDark);
        document.body.classList.toggle('mud-theme-light', !isDark);
    },
    
    init: function () {
        const theme = this.getTheme();
        this.applyTheme(theme);
    }
};

// Aplicar tema na inicialização
document.addEventListener('DOMContentLoaded', function() {
    window.themeService.init();
});
