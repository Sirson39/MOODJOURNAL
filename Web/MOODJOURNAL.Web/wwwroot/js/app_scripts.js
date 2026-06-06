var quillEditors = {};

window.initQuill = (elementId, content, dotNetRef, readOnly) => {
    var container = document.getElementById(elementId);
    if (!container) return;

    var quill = new Quill(container, {
        modules: {
            toolbar: readOnly ? false : [
                [{ header: [1, 2, false] }],
                ['bold', 'italic', 'underline'],
                ['list', 'bullet'],
                ['link', 'clean']
            ]
        },
        placeholder: readOnly ? '' : 'Start writing your thoughts...',
        theme: 'snow',
        readOnly: readOnly || false
    });

    if (content) {
        quill.root.innerHTML = content;
    }

    if (dotNetRef) {
        quill.on('text-change', function () {
            var text = quill.getText();
            dotNetRef.invokeMethodAsync('UpdateWordCount', text);
        });
    }

    quillEditors[elementId] = quill;
};

window.getQuillContent = (elementId) => {
    return quillEditors[elementId] ? quillEditors[elementId].root.innerHTML : "";
};

window.getQuillText = (elementId) => {
    return quillEditors[elementId] ? quillEditors[elementId].root.innerText : "";
};

window.createPieChart = (elementId, labels, data, colors) => {
    var canvas = document.getElementById(elementId);
    if (!canvas || typeof Chart === 'undefined' || !canvas.getContext) {
        return;
    }

    var ctx = canvas.getContext('2d');
    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors,
                borderWidth: 0
            }]
        },
        options: {
            cutout: '70%',
            plugins: { legend: { position: 'bottom' } }
        }
    });
};

window.createLineChart = (elementId, labels, data) => {
    var canvas = document.getElementById(elementId);
    if (!canvas || typeof Chart === 'undefined' || !canvas.getContext) {
        return;
    }

    var ctx = canvas.getContext('2d');
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Words',
                data: data,
                borderColor: '#C46210',
                backgroundColor: 'rgba(196, 98, 16, 0.1)',
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });
};

window.createBarChart = (elementId, labels, data) => {
    var canvas = document.getElementById(elementId);
    if (!canvas || typeof Chart === 'undefined' || !canvas.getContext) {
        return;
    }

    var ctx = canvas.getContext('2d');
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Usage',
                data: data,
                backgroundColor: '#F9EBE0',
                borderRadius: 8
            }]
        },
        options: {
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });
};

window.downloadFile = (fileName, base64Data, mimeType) => {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = `data:${mimeType || 'application/octet-stream'};base64,${base64Data}`;
    document.body.appendChild(link);
    link.click();
    link.remove();
};

/* --- Professional UI Helpers --- */

window.showToast = (message, type) => {
    var container = document.getElementById('toast-container');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toast-container';
        container.className = 'toast-container';
        document.body.appendChild(container);
    }

    var toast = document.createElement('div');
    toast.className = `toast-message ${type || 'success'}`;
    var icon = type === 'error' ? 'ph-warning-circle' : 'ph-check-circle';

    toast.innerHTML = `<i class="ph-fill ${icon}" style="font-size: 1.5rem;"></i><span style="font-weight: 500;">${message}</span>`;

    container.appendChild(toast);

    // Auto remove after 3.5s
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(50px)';
        toast.style.transition = 'all 0.4s ease';
        setTimeout(() => toast.remove(), 400);
    }, 3500);
};

window.customConfirm = (title, text, confirmText, iconEmoji) => {
    return new Promise((resolve) => {
        var overlay = document.createElement('div');
        overlay.className = 'modal-overlay';

        var card = document.createElement('div');
        card.className = 'modal-card';

        card.innerHTML = `
            <div class="modal-icon">${iconEmoji || '🗑️'}</div>
            <h3 class="modal-title brand-font">${title}</h3>
            <p class="modal-text">${text}</p>
            <div class="modal-actions">
                <button class="btn-secondary" id="confirm-cancel">Cancel</button>
                <button class="btn-primary" id="confirm-ok" style="background: var(--mood-negative); color: var(--mood-negative-text); border: none; padding: 0.75rem 2rem;">${confirmText || 'Confirm'}</button>
            </div>
        `;

        overlay.appendChild(card);
        document.body.appendChild(overlay);

        document.getElementById('confirm-cancel').onclick = () => {
            overlay.remove();
            resolve(false);
        };

        document.getElementById('confirm-ok').onclick = () => {
            overlay.remove();
            resolve(true);
        };
    });
};
