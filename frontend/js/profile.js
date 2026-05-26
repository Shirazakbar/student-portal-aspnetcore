//const API_URL = 'http://localhost:5275/api';
const API_URL = 'http://localhost:5000/api';
const userStr = localStorage.getItem('user');
if (!userStr) {
    window.location.href = 'index.html';
}

const user = JSON.parse(userStr);

document.addEventListener('DOMContentLoaded', () => {
    const currentProfilePic = document.getElementById('currentProfilePic');
    const profilePicInput = document.getElementById('profilePicInput');
    const newPassword = document.getElementById('newPassword');
    const saveProfileBtn = document.getElementById('saveProfileBtn');
    const profileMessage = document.getElementById('profileMessage');

    // Load existing profile pic if present
    if (user.profilePictureUrl) {
        currentProfilePic.src = `http://localhost:5275${user.profilePictureUrl}`;
    }

    // Preview newly selected image
    profilePicInput.addEventListener('change', () => {
        const file = profilePicInput.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = e => currentProfilePic.src = e.target.result;
            reader.readAsDataURL(file);
        }
    });

    saveProfileBtn.addEventListener('click', async () => {
        const formData = new FormData();
        formData.append('userId', user.id);

        if (newPassword.value) {
            formData.append('password', newPassword.value);
        }

        if (profilePicInput.files[0]) {
            formData.append('profilePicture', profilePicInput.files[0]);
        }

        saveProfileBtn.disabled = true;
        profileMessage.textContent = 'Saving...';
        profileMessage.style.color = 'var(--text-secondary)';

        try {
            const res = await fetch(`${API_URL}/auth/profile`, {
                method: 'PUT',
                body: formData // No Content-Type header so browser sets multipart boundary automatically
            });

            if (res.ok) {
                const updatedUser = await res.json();
                localStorage.setItem('user', JSON.stringify(updatedUser)); // Update local context
                profileMessage.textContent = 'Profile updated successfully!';
                profileMessage.style.color = '#34d399';
                newPassword.value = '';
            } else {
                profileMessage.textContent = 'Failed to update profile.';
                profileMessage.style.color = 'var(--error)';
            }
        } catch (err) {
            profileMessage.textContent = 'Server error. Is the backend running?';
            profileMessage.style.color = 'var(--error)';
        } finally {
            saveProfileBtn.disabled = false;
        }
    });
});
