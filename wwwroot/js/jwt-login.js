
let accessToken = null;

const status = document.getElementById('status');
const result = document.getElementById('result');

document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    accessToken = null;

    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                username: document.getElementById('username').value,
                password: document.getElementById('password').value
            })
        });

        const data = await response.json();

        if (!response.ok) {
            status.textContent = 'Đăng nhập thất bại (HTTP ' + response.status + ')';
            result.textContent = JSON.stringify(data, null, 2);
            return;
        }

        accessToken = data.accessToken;

        status.textContent = 'Đăng nhập thành công: '
            + data.username + ' | Role: ' + data.role;

        // Chuyển sang Home sau khi đăng nhập thành công
        window.location.href = '/Home/Index';

    } catch (err) {
        status.textContent = 'Lỗi kết nối: ' + err.message;
    }
});

document.getElementById('logout').addEventListener('click', () => {
    accessToken = null;
    status.textContent = 'Đã đăng xuất';
    result.textContent = '';
});

for (const route of ['me', 'user', 'admin']) {
    document.getElementById(route).addEventListener('click', async () => {
        try {
            const response = await fetch('/api/auth/' + route, {
                headers: accessToken
                    ? { 'Authorization': 'Bearer ' + accessToken }
                    : {}
            });

            const body = await response.text();

            result.textContent =
                'GET /api/auth/' + route +
                ' — HTTP ' + response.status + '\n' + body;

        } catch (err) {
            result.textContent = err.message;
        }
    });
}
