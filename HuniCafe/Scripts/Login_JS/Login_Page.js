document.addEventListener("DOMContentLoaded", function () {
    const loginForm = document.getElementById("loginForm");
    const usernameInput = document.getElementById("username");
    const passwordInput = document.getElementById("password");

    if (loginForm) {
        loginForm.addEventListener("submit", function (e) {
            if (usernameInput.value.trim() === "" || passwordInput.value.trim() === "") {
                e.preventDefault();
                alert("Vui lòng không để trống Tên đăng nhập và Mật khẩu!");
            }
        });
    }
});