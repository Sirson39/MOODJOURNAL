var quillEditors = {};

window.initQuill = (elementId, content, dotNetRef) => {
    var container = document.getElementById(elementId);
    if (!container) return;

    var quill = new Quill(container, {
        modules: {
            toolbar: [
                [{ header: [1, 2, false] }],
                ['bold', 'italic', 'underline'],
                ['list', 'bullet'],
                ['link', 'clean']
            ]
        },
        placeholder: 'Start writing your thoughts...',
        theme: 'snow'
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
    var ctx = document.getElementById(elementId).getContext('2d');
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
    var ctx = document.getElementById(elementId).getContext('2d');
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
    var ctx = document.getElementById(elementId).getContext('2d');
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
