document.addEventListener('DOMContentLoaded', function () {
    var registerForm = document.getElementById('registerForm');

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            var pwd = document.getElementById('txtPassword').value;
            var confirmPwd = document.getElementById('txtConfirmPassword').value;
            var errorBox = document.getElementById('js-error');

            // Reset ẩn khung lỗi
            errorBox.style.display = 'none';
            errorBox.innerText = '';

            // Kiểm tra độ dài mật khẩu
            if (pwd.length < 6) {
                e.preventDefault();
                errorBox.innerText = 'Mật khẩu phải có ít nhất 6 ký tự!';
                errorBox.style.display = 'block';
                return;
            }

            // Kiểm tra khớp mật khẩu
            if (pwd !== confirmPwd) {
                e.preventDefault();
                errorBox.innerText = 'Mật khẩu xác nhận không trùng khớp!';
                errorBox.style.display = 'block';
                return;
            }
        });
    }
});