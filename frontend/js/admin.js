const API_URL = 'http://localhost:5275/api';
const adminSections = ['Overview', 'Notices', 'Videos', 'Assignments'];

const userStr = localStorage.getItem('user');
if (!userStr) {
    window.location.href = 'index.html';
}

const user = JSON.parse(userStr);
if (!user.role || user.role.toLowerCase() !== 'admin') {
    window.location.href = 'main.html';
}

function getAdminHeaders() {
    // For hardcoded admin login, send special header
    if (user.isAdmin === true || user.id === 'admin-001') {
        return {
            'X-Admin-Auth': 'admin-001'
        };
    }
    
    // For database users with admin role
    return {
        'X-User-Id': user.id.toString(),
        'X-User-Email': user.email
    };
}

function showSection(sectionId) {
    adminSections.forEach(name => {
        const element = document.getElementById(`section${name}`);
        if (element) {
            element.style.display = name === sectionId ? 'block' : 'none';
        }
        const nav = document.getElementById(`nav${name}`);
        if (nav) {
            nav.classList.toggle('active', name === sectionId);
        }
    });
}

async function loadOverview() {
    const [notices, videos, assignments] = await Promise.all([
        fetch(`${API_URL}/notices`).then(r => r.ok ? r.json() : []),
        fetch(`${API_URL}/videos`).then(r => r.ok ? r.json() : []),
        fetch(`${API_URL}/assignments`).then(r => r.ok ? r.json() : [])
    ]);

    document.getElementById('noticeCount').textContent = `${notices.length} notices`;
    document.getElementById('videoCount').textContent = `${videos.length} videos`;
    document.getElementById('assignmentCount').textContent = `${assignments.length} assignments`;
}

async function loadNotices() {
    const res = await fetch(`${API_URL}/notices`);
    if (!res.ok) return;
    const notices = await res.json();
    const container = document.getElementById('noticeList');
    container.innerHTML = notices.length ? notices.map(createNoticeCard).join('') : '<p style="text-align:center; color:var(--text-secondary);">No notices yet.</p>';
}

function createNoticeCard(notice) {
    return `
        <div class="post-card" id="notice-${notice.id}">
            <div class="post-header" style="align-items:flex-start; gap:1rem;">
                <div>
                    <h4 style="margin-bottom:0.5rem;">${escapeHTML(notice.title)}</h4>
                    <span style="color:var(--text-secondary);">Created: ${new Date(notice.createdAt).toLocaleString()}</span>
                </div>
                <div style="display:flex; gap:0.75rem; flex-wrap:wrap; justify-content:flex-end; align-items:flex-start;">
                    <button class="btn-primary" style="background:rgba(99,102,241,0.16); color:white; border:none;" onclick="editNotice(${notice.id})">Edit</button>
                    <button class="btn-primary" style="background:rgba(239,68,68,0.18); color:white; border:none;" onclick="deleteNotice(${notice.id})">Delete</button>
                </div>
            </div>
            <p style="line-height:1.6; color:#e2e8f0;">${escapeHTML(notice.content).replace(/\n/g, '<br>')}</p>
        </div>
    `;
}

async function loadVideos() {
    const res = await fetch(`${API_URL}/videos`);
    if (!res.ok) return;
    const videos = await res.json();
    const container = document.getElementById('videoList');
    container.innerHTML = videos.length ? videos.map(createVideoCard).join('') : '<p style="text-align:center; color:var(--text-secondary);">No videos yet.</p>';
}

function createVideoCard(video) {
    return `
        <div class="post-card" id="video-${video.id}">
            <div class="post-header" style="align-items:flex-start; gap:1rem;">
                <div>
                    <h4 style="margin-bottom:0.5rem;">${escapeHTML(video.title)}</h4>
                    <span style="color:var(--text-secondary);">Created: ${new Date(video.createdAt).toLocaleString()}</span>
                </div>
                <div style="display:flex; gap:0.75rem; flex-wrap:wrap; justify-content:flex-end; align-items:flex-start;">
                    <button class="btn-primary" style="background:rgba(99,102,241,0.16); color:white; border:none;" onclick="editVideo(${video.id})">Edit</button>
                    <button class="btn-primary" style="background:rgba(239,68,68,0.18); color:white; border:none;" onclick="deleteVideo(${video.id})">Delete</button>
                </div>
            </div>
            <p style="line-height:1.6; color:#e2e8f0;">${escapeHTML(video.description || 'No description provided.')}</p>
            ${video.videoUrl ? `<p><a href="${escapeHTML(video.videoUrl)}" target="_blank" style="color:var(--primary);">Open Video URL</a></p>` : ''}
            ${video.fileUrl ? `<p><a href="http://localhost:5275${escapeHTML(video.fileUrl)}" target="_blank" style="color:var(--primary);">Download uploaded file</a></p>` : ''}
        </div>
    `;
}

async function loadAssignments() {
    const res = await fetch(`${API_URL}/assignments`);
    if (!res.ok) return;
    const assignments = await res.json();
    const container = document.getElementById('assignmentList');
    container.innerHTML = assignments.length ? assignments.map(createAssignmentCard).join('') : '<p style="text-align:center; color:var(--text-secondary);">No assignments yet.</p>';
}

function createAssignmentCard(assignment) {
    return `
        <div class="post-card" id="assignment-${assignment.id}">
            <div class="post-header" style="align-items:flex-start; gap:1rem;">
                <div>
                    <h4 style="margin-bottom:0.5rem;">${escapeHTML(assignment.title)}</h4>
                    <span style="color:var(--text-secondary);">Created: ${new Date(assignment.createdAt).toLocaleString()}</span>
                </div>
                <div style="display:flex; gap:0.75rem; flex-wrap:wrap; justify-content:flex-end; align-items:flex-start;">
                    <button class="btn-primary" style="background:rgba(99,102,241,0.16); color:white; border:none;" onclick="editAssignment(${assignment.id})">Edit</button>
                    <button class="btn-primary" style="background:rgba(239,68,68,0.18); color:white; border:none;" onclick="deleteAssignment(${assignment.id})">Delete</button>
                </div>
            </div>
            <p style="line-height:1.6; color:#e2e8f0;">${escapeHTML(assignment.description || 'No description provided.')}</p>
            ${assignment.attachmentUrl ? `<p><a href="http://localhost:5275${escapeHTML(assignment.attachmentUrl)}" target="_blank" style="color:var(--primary);">Download attachment</a></p>` : ''}
        </div>
    `;
}

async function createNotice() {
    const title = document.getElementById('noticeTitle').value.trim();
    const content = document.getElementById('noticeContent').value.trim();
    const message = document.getElementById('noticeMessage');

    if (!title || !content) {
        message.textContent = 'Please provide both title and content.';
        return;
    }

    try {
        const res = await fetch(`${API_URL}/admin/notices`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                ...getAdminHeaders()
            },
            body: JSON.stringify({ title, content })
        });

        if (res.ok) {
            message.style.color = '#34d399';
            message.textContent = 'Notice saved successfully.';
            document.getElementById('noticeTitle').value = '';
            document.getElementById('noticeContent').value = '';
            loadNotices();
            loadOverview();
        } else {
            const errorText = await res.text();
            message.style.color = '#ef4444';
            message.textContent = `Error: ${errorText || 'Unable to create notice. Status: ' + res.status}`;
        }
    } catch (err) {
        message.style.color = '#ef4444';
        message.textContent = `Error: ${err.message}`;
    }
}

async function updateNotice(id, title, content) {
    await fetch(`${API_URL}/admin/notices/${id}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            ...getAdminHeaders()
        },
        body: JSON.stringify({ title, content })
    });
}

async function deleteNotice(id) {
    if (!confirm('Delete this notice?')) return;
    const res = await fetch(`${API_URL}/admin/notices/${id}`, {
        method: 'DELETE',
        headers: getAdminHeaders()
    });
    if (res.ok) {
        loadNotices();
        loadOverview();
    }
}

async function createVideo() {
    const title = document.getElementById('videoTitle').value.trim();
    const description = document.getElementById('videoDescription').value.trim();
    const videoUrl = document.getElementById('videoUrl').value.trim();
    const file = document.getElementById('videoFile').files[0];
    const message = document.getElementById('videoMessage');

    if (!title) {
        message.textContent = 'Video title is required.';
        return;
    }

    try {
        const formData = new FormData();
        formData.append('title', title);
        formData.append('description', description);
        if (videoUrl) formData.append('videoUrl', videoUrl);
        if (file) formData.append('file', file);

        const res = await fetch(`${API_URL}/admin/videos`, {
            method: 'POST',
            headers: getAdminHeaders(),
            body: formData
        });

        if (res.ok) {
            message.style.color = '#34d399';
            message.textContent = 'Video saved successfully.';
            document.getElementById('videoTitle').value = '';
            document.getElementById('videoDescription').value = '';
            document.getElementById('videoUrl').value = '';
            document.getElementById('videoFile').value = '';
            loadVideos();
            loadOverview();
        } else {
            const errorText = await res.text();
            message.style.color = '#ef4444';
            message.textContent = `Error: ${errorText || 'Unable to save video. Status: ' + res.status}`;
        }
    } catch (err) {
        message.style.color = '#ef4444';
        message.textContent = `Error: ${err.message}`;
    }
}

async function deleteVideo(id) {
    if (!confirm('Delete this video?')) return;
    const res = await fetch(`${API_URL}/admin/videos/${id}`, {
        method: 'DELETE',
        headers: getAdminHeaders()
    });
    if (res.ok) {
        loadVideos();
        loadOverview();
    }
}

async function createAssignment() {
    const title = document.getElementById('assignmentTitle').value.trim();
    const description = document.getElementById('assignmentDescription').value.trim();
    const dueDate = document.getElementById('assignmentDueDate').value;
    const file = document.getElementById('assignmentFile').files[0];
    const message = document.getElementById('assignmentMessage');

    if (!title || !dueDate) {
        message.textContent = 'Assignment title and due date are required.';
        return;
    }

    try {
        const formData = new FormData();
        formData.append('title', title);
        formData.append('description', description);
        formData.append('dueDate', dueDate);
        if (file) formData.append('attachment', file);

        const res = await fetch(`${API_URL}/admin/assignments`, {
            method: 'POST',
            headers: getAdminHeaders(),
            body: formData
        });

        if (res.ok) {
            message.style.color = '#34d399';
            message.textContent = 'Assignment saved successfully.';
            document.getElementById('assignmentTitle').value = '';
            document.getElementById('assignmentDescription').value = '';
            document.getElementById('assignmentDueDate').value = '';
            document.getElementById('assignmentFile').value = '';
            loadAssignments();
            loadOverview();
        } else {
            const errorText = await res.text();
            message.style.color = '#ef4444';
            message.textContent = `Error: ${errorText || 'Unable to save assignment. Status: ' + res.status}`;
        }
    } catch (err) {
        message.style.color = '#ef4444';
        message.textContent = `Error: ${err.message}`;
    }
}

async function deleteAssignment(id) {
    if (!confirm('Delete this assignment?')) return;
    const res = await fetch(`${API_URL}/admin/assignments/${id}`, {
        method: 'DELETE',
        headers: getAdminHeaders()
    });
    if (res.ok) {
        loadAssignments();
        loadOverview();
    }
}

function logout() {
    localStorage.removeItem('user');
    window.location.href = '../index.html';
}

function escapeHTML(str) {
    return String(str).replace(/[&<>'"]/g, tag => ({
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        "'": '&#39;',
        '"': '&quot;'
    }[tag]));
}

function bindEvents() {
    document.getElementById('navHome').addEventListener('click', (e) => { e.preventDefault(); showSection('Overview'); });
    document.getElementById('navNotices').addEventListener('click', (e) => { e.preventDefault(); showSection('Notices'); loadNotices(); });
    document.getElementById('navVideos').addEventListener('click', (e) => { e.preventDefault(); showSection('Videos'); loadVideos(); });
    document.getElementById('navAssignments').addEventListener('click', (e) => { e.preventDefault(); showSection('Assignments'); loadAssignments(); });

    document.getElementById('createNoticeBtn').addEventListener('click', createNotice);
    document.getElementById('createVideoBtn').addEventListener('click', createVideo);
    document.getElementById('createAssignmentBtn').addEventListener('click', createAssignment);
}

window.editNotice = async function (id) {
    const notice = await fetch(`${API_URL}/notices/${id}`).then(r => r.ok ? r.json() : null);
    if (!notice) return;
    const title = prompt('Edit notice title', notice.title);
    const content = prompt('Edit notice content', notice.content);
    if (title === null || content === null) return;
    await updateNotice(id, title, content);
    loadNotices();
};

window.editVideo = async function (id) {
    const video = await fetch(`${API_URL}/videos/${id}`).then(r => r.ok ? r.json() : null);
    if (!video) return;
    const title = prompt('Edit video title', video.title);
    const description = prompt('Edit description', video.description || '');
    const videoUrl = prompt('Edit video URL', video.videoUrl || '');
    if (title === null || description === null || videoUrl === null) return;
    const formData = new FormData();
    formData.append('title', title);
    formData.append('description', description);
    formData.append('videoUrl', videoUrl);
    await fetch(`${API_URL}/admin/videos/${id}`, { method: 'PUT', headers: getAdminHeaders(), body: formData });
    loadVideos();
};

window.editAssignment = async function (id) {
    const assignment = await fetch(`${API_URL}/assignments/${id}`).then(r => r.ok ? r.json() : null);
    if (!assignment) return;
    const title = prompt('Edit assignment title', assignment.title);
    const description = prompt('Edit description', assignment.description || '');
    if (title === null || description === null) return;
    const formData = new FormData();
    formData.append('title', title);
    formData.append('description', description);
    await fetch(`${API_URL}/admin/assignments/${id}`, { method: 'PUT', headers: getAdminHeaders(), body: formData });
    loadAssignments();
};

window.onload = () => {
    document.getElementById('userName').textContent = user.name;
    bindEvents();
    showSection('Overview');
    loadOverview();
};
