const API_URL = '/api';
//const API_URL = 'http://localhost:5275/api';   //Change this maybe?

// Switch between Student and Admin login modes
function switchAuthMode(mode) {
    const studentSection = document.getElementById('studentSection');
    const adminSection = document.getElementById('adminSection');
    const studentBtn = document.getElementById('studentModeBtn');
    const adminBtn = document.getElementById('adminModeBtn');

    if (mode === 'student') {
        studentSection.classList.add('active');
        adminSection.classList.remove('active');
        studentBtn.classList.add('active');
        adminBtn.classList.remove('active');
    } else if (mode === 'admin') {
        adminSection.classList.add('active');
        studentSection.classList.remove('active');
        adminBtn.classList.add('active');
        studentBtn.classList.remove('active');
    }
}

function toggleForms() {
    const loginForm = document.getElementById('loginForm');
    const signupForm = document.getElementById('signupForm');

    if (loginForm.classList.contains('active')) {
        loginForm.classList.remove('active');
        signupForm.classList.add('active');
    } else {
        signupForm.classList.remove('active');
        loginForm.classList.add('active');
    }
}

document.addEventListener('DOMContentLoaded', () => {

    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const email = document.getElementById('loginEmail').value;
            const password = document.getElementById('loginPassword').value;
            const errorDiv = document.getElementById('loginError');

            try {
                const res = await fetch(`${API_URL}/auth/login`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, password })
                });

                if (res.ok) {
                    const data = await res.json();
                    localStorage.setItem('user', JSON.stringify(data));
                    const destination = data.role && data.role.toLowerCase() === 'admin' ? 'admin/dashboard.html' : 'main.html';
                    window.location.href = destination;
                } else {
                    const errText = await res.text();
                    errorDiv.textContent = errText || 'Login failed';
                }
            } catch (err) {
                errorDiv.textContent = 'Server error. Is the backend running?';
            }
        });
    }

    const signupForm = document.getElementById('signupForm');
    if (signupForm) {
        signupForm.addEventListener('submit', async (e) => {
            e.preventDefault();
            const name = document.getElementById('signupName').value;
            const email = document.getElementById('signupEmail').value;
            const password = document.getElementById('signupPassword').value;
            const errorDiv = document.getElementById('signupError');

            try {
                const res = await fetch(`${API_URL}/auth/signup`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ name, email, password })
                });

                if (res.ok) {
                    const data = await res.json();
                    localStorage.setItem('user', JSON.stringify(data));
                    window.location.href = 'main.html';
                } else {
                    const errText = await res.text();
                    errorDiv.textContent = errText || 'Signup failed';
                }
            } catch (err) {
                errorDiv.textContent = 'Server error. Is the backend running?';
            }
        });
    }

    // Admin Login Form Handler
    const adminLoginForm = document.getElementById('adminLoginForm');
    if (adminLoginForm) {
        adminLoginForm.addEventListener('submit', (e) => {
            e.preventDefault();
            const username = document.getElementById('adminUsername').value;
            const password = document.getElementById('adminPassword').value;
            const errorDiv = document.getElementById('adminError');

            // Check hardcoded admin credentials
            if (username === 'admin' && password === 'admin') {
                const adminData = {
                    id: 'admin-001',
                    name: 'Admin',
                    email: 'admin@portal.edu',
                    role: 'Admin',
                    isAdmin: true
                };
                localStorage.setItem('user', JSON.stringify(adminData));
                window.location.href = 'admin/dashboard.html';
            } else {
                errorDiv.textContent = 'Invalid admin credentials. Username and password should be "admin".';
            }
        });
    }
});
