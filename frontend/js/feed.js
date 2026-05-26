//const API_URL = 'http://localhost:5275/api';
const API_URL = 'http://localhost:5000/api';

// Authentication check
const userStr = localStorage.getItem('user');
if (!userStr) {
    window.location.href = 'index.html';
}

const user = JSON.parse(userStr);

document.addEventListener('DOMContentLoaded', () => {
    // Set user info in header
    document.getElementById('userName').textContent = user.name;

    // Check if there's a tab to navigate to from localStorage
    const selectedTab = localStorage.getItem('selectedTab');
    if (selectedTab) {
        localStorage.removeItem('selectedTab');
        switchTab(selectedTab);
    }

    // Load initial posts
    loadPosts();

    // Event listener for creating a post
    const submitPost = document.getElementById('submitPost');
    submitPost.addEventListener('click', async () => {
        const content = document.getElementById('postContent').value.trim();
        const errorDiv = document.getElementById('postError');

        if (!content) {
            errorDiv.textContent = 'Post cannot be empty.';
            return;
        }

        try {
            const res = await fetch(`${API_URL}/feed`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ userId: user.id, content })
            });

            if (res.ok) {
                document.getElementById('postContent').value = '';
                errorDiv.textContent = '';
                loadPosts(); // Reload posts
            } else {
                errorDiv.textContent = 'Failed to create post.';
            }
        } catch (err) {
            errorDiv.textContent = 'Server error.';
        }
    });

    // Tab switching functionality
    const navTabs = document.querySelectorAll('.nav-tab');
    navTabs.forEach(tab => {
        tab.addEventListener('click', (e) => {
            e.preventDefault();
            const tabName = tab.getAttribute('data-tab');
            switchTab(tabName);
        });
    });

    // Load assignments and videos on page load
    loadAssignments();
    loadVideos();
    loadNoticeboard();
});

// Tab Switching Function
function switchTab(tabName) {
    // Hide all tabs
    const tabContents = document.querySelectorAll('.tab-content');
    tabContents.forEach(tab => tab.classList.remove('active'));

    // Remove active class from all nav links
    const navTabs = document.querySelectorAll('.nav-tab');
    navTabs.forEach(tab => tab.classList.remove('active'));

    // Show selected tab
    const selectedTab = document.getElementById(`${tabName}-tab`);
    if (selectedTab) {
        selectedTab.classList.add('active');
    }

    // Add active class to clicked nav link
    const activeTab = document.querySelector(`[data-tab="${tabName}"]`);
    if (activeTab) {
        activeTab.classList.add('active');
    }

    // Update title
    const titles = {
        'home': 'Student Feed',
        'assignments': 'Assignments',
        'videos': 'Videos',
        'noticeboard': 'Noticeboard'
    };
    document.getElementById('tabTitle').textContent = titles[tabName] || 'Student Feed';
}

async function loadPosts() {
    try {
        const res = await fetch(`${API_URL}/feed`);
        if (res.ok) {
            const posts = await res.json();
            renderPosts(posts);
        }
    } catch (err) {
        console.error('Error loading posts:', err);
    }
}

function renderPosts(posts) {
    const feedList = document.getElementById('feedList');
    feedList.innerHTML = '';

    if (posts.length === 0) {
        feedList.innerHTML = '<p style="text-align:center; color:var(--text-secondary);">No updates yet. Be the first to post!</p>';
        return;
    }

    posts.forEach(post => {
        const date = new Date(post.createdAt);
        const dateStr = date.toLocaleString();

        const profilePicSrc = post.user.profilePictureUrl ? `http://localhost:5275${post.user.profilePictureUrl}` : 'https://via.placeholder.com/30';

        const card = document.createElement('div');
        card.className = 'post-card';
        card.innerHTML = `
            <div class="post-header">
                <div style="display:flex; gap:10px; align-items:center;">
                    <img src="${profilePicSrc}" alt="Avatar" style="width:30px; height:30px; border-radius:50%; object-fit:cover;">
                    <span class="post-author">${escapeHTML(post.user.name)}</span>
                </div>
                <span class="post-time">${dateStr}</span>
            </div>
            <div class="post-content">${escapeHTML(post.content).replace(/\n/g, '<br>')}</div>
            <div class="post-actions" style="margin-top: 1rem; border-top: 1px solid var(--glass-border); padding-top: 10px; justify-content: flex-start; gap: 20px;">
                <button onclick="reactToPost(${post.id}, true)" class="btn-react" style="background:none; border:none; color:white; font-size:1.1rem; cursor:pointer;"><i class="fas fa-thumbs-up" style="color:var(--primary);"></i> <span style="font-size:0.95rem; margin-left:5px;">${post.likes || 0}</span></button>
                <button onclick="reactToPost(${post.id}, false)" class="btn-react" style="background:none; border:none; color:white; font-size:1.1rem; cursor:pointer;"><i class="fas fa-thumbs-down" style="color:var(--secondary);"></i> <span style="font-size:0.95rem; margin-left:5px;">${post.dislikes || 0}</span></button>
            </div>
        `;
        feedList.appendChild(card);
    });
}

async function reactToPost(postId, isLike) {
    try {
        const res = await fetch(`${API_URL}/feed/${postId}/react`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ userId: user.id, isLike })
        });
        if (res.ok) {
            loadPosts(); // Reload silently
        }
    } catch (err) {
        console.error('Failed to react:', err);
    }
}

function logout() {
    localStorage.removeItem('user');
    window.location.href = 'index.html';
}

function escapeHTML(str) {
    return str.replace(/[&<>'"]/g,
        tag => ({
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            "'": '&#39;',
            '"': '&quot;'
        }[tag])
    );
}

// Assignments Tab Functions
async function loadAssignments() {
    try {
        const res = await fetch(`${API_URL}/assignments`);
        if (res.ok) {
            const assignments = await res.json();
            renderAssignments(assignments);
        }
    } catch (err) {
        console.error('Error loading assignments:', err);
    }
}

function renderAssignments(assignments) {
    const container = document.getElementById('assignmentsContainer');
    container.innerHTML = '';

    if (assignments.length === 0) {
        container.innerHTML = '<p style="text-align:center; color:var(--text-secondary); width:100%;">No assignments yet.</p>';
        return;
    }

    assignments.forEach(assignment => {
        const deadline = new Date(assignment.dueDate).toLocaleDateString();
        const box = document.createElement('div');
        box.className = 'assignment-box';
        box.innerHTML = `
            <div class="assignment-title">${escapeHTML(assignment.title)}</div>
            <div class="assignment-content">
                <div style="text-align:center;">
                    <i class="fas fa-file-alt" style="font-size:3rem; color:var(--primary); margin-bottom:1rem;"></i>
                </div>
            </div>
            <div class="assignment-deadline"><strong>Deadline:</strong> ${deadline}</div>
            <button class="assignment-details-btn" onclick="openAssignmentModal(${assignment.id}, '${escapeHTML(assignment.title).replace(/'/g, "\\'")}', '${escapeHTML(assignment.description).replace(/'/g, "\\'")}')">Details</button>
        `;
        container.appendChild(box);
    });
}

function openAssignmentModal(id, title, description) {
    const modal = document.getElementById('assignmentModal');
    document.getElementById('modalTitle').textContent = title;
    document.getElementById('modalDescription').textContent = description;
    modal.classList.add('active');
}

function closeAssignmentModal() {
    const modal = document.getElementById('assignmentModal');
    modal.classList.remove('active');
}

// Videos Tab Functions
async function loadVideos() {
    try {
        const res = await fetch(`${API_URL}/videos`);
        if (res.ok) {
            const videos = await res.json();
            renderVideos(videos);
        }
    } catch (err) {
        console.error('Error loading videos:', err);
    }
}

function renderVideos(videos) {
    const container = document.getElementById('videosContainer');
    container.innerHTML = '';

    if (videos.length === 0) {
        container.innerHTML = '<p style="color:var(--text-secondary);">No videos yet.</p>';
        return;
    }

    videos.forEach((video, index) => {
        const box = document.createElement('div');
        box.className = 'video-box';
        box.innerHTML = `
            <div class="video-number">${index + 1}</div>
            <div class="video-title">${escapeHTML(video.title)}</div>
            <div class="video-description">${escapeHTML(video.description || 'No description')}</div>
        `;
        container.appendChild(box);
    });
}

// Noticeboard Tab Functions
async function loadNoticeboard() {
    try {
        const res = await fetch(`${API_URL}/notices`);
        if (res.ok) {
            const notices = await res.json();
            renderNoticeboard(notices);
        }
    } catch (err) {
        console.error('Error loading noticeboard:', err);
    }
}

function renderNoticeboard(notices) {
    const container = document.getElementById('noticeboardText');
    container.innerHTML = '';

    if (notices.length === 0) {
        container.textContent = 'No notices available.';
        return;
    }

    const allText = notices.map(notice => escapeHTML(notice.title)).join(' • ');
    container.textContent = allText;
}
