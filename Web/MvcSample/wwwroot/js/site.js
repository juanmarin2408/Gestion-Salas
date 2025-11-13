document.addEventListener("DOMContentLoaded", function () {
    var quickItems = document.querySelectorAll(".quick-access-item");
    var emailInput = document.getElementById("Email");
    var passwordInput = document.getElementById("Password");

    if (!quickItems || !emailInput || !passwordInput) return;

    quickItems.forEach(function (item) {
        item.addEventListener("click", function () {
            var email = item.getAttribute("data-email");
            var password = item.getAttribute("data-password");

            if (email) emailInput.value = email;
            if (password) passwordInput.value = password;

            // Llevar el foco al botón de login
            var loginButton = document.querySelector(".login-button");
            if (loginButton) loginButton.focus();
        });
    });
});
