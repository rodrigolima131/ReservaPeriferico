window.themeService = {
    getTheme: function () {
        return localStorage.getItem('theme') || 'light';
    },
    
    setTheme: function (theme) {
        localStorage.setItem('theme', theme);
    },
    
    isDarkMode: function () {
        return this.getTheme() === 'dark';
    }
};
