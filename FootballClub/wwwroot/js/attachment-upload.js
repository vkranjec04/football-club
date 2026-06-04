/**
 * Attachment Upload Component
 * Handles file uploads, downloads, and deletions via AJAX
 */

const AttachmentUpload = (() => {
    let config = {
        entityType: '',
        entityId: 0,
        maxFileSize: 10 * 1024 * 1024 // 10 MB
    };

    const ALLOWED_MIME_TYPES = [
        'application/pdf',
        'image/jpeg',
        'image/png',
        'image/gif',
        'image/webp',
        'text/plain'
    ];

    const FILE_ICONS = {
        'application/pdf': '📄',
        'image/jpeg': '🖼️',
        'image/png': '🖼️',
        'image/gif': '🖼️',
        'image/webp': '🖼️',
        'text/plain': '📝'
    };

    /**
     * Initialize the upload component
     */
    function init(elementId, options) {
        config = { ...config, ...options };
        
        const dropzone = document.getElementById(elementId);
        if (!dropzone) {
            console.error(`Dropzone element with id "${elementId}" not found`);
            return;
        }

        setupEventListeners(dropzone);
        loadExistingFiles();
    }

    /**
     * Setup event listeners for dropzone
     */
    function setupEventListeners(dropzone) {
        const fileInput = dropzone.querySelector('#fileInput');

        // Click to browse
        dropzone.addEventListener('click', () => fileInput.click());

        // File input change
        fileInput.addEventListener('change', (e) => {
            handleFiles(e.target.files);
            fileInput.value = ''; // Reset input
        });

        // Drag and drop
        dropzone.addEventListener('dragover', (e) => {
            e.preventDefault();
            e.stopPropagation();
            dropzone.classList.add('dragover');
        });

        dropzone.addEventListener('dragleave', (e) => {
            e.preventDefault();
            e.stopPropagation();
            dropzone.classList.remove('dragover');
        });

        dropzone.addEventListener('drop', (e) => {
            e.preventDefault();
            e.stopPropagation();
            dropzone.classList.remove('dragover');
            handleFiles(e.dataTransfer.files);
        });
    }

    /**
     * Handle file selection
     */
    function handleFiles(files) {
        if (files.length === 0) return;

        const validFiles = Array.from(files).filter(file => {
            if (!ALLOWED_MIME_TYPES.includes(file.type)) {
                showAlert(`File "${file.name}" has unsupported type. Allowed: PDF, PNG, JPG, GIF, WebP, TXT`, 'danger');
                return false;
            }

            if (file.size > config.maxFileSize) {
                const maxMB = (config.maxFileSize / (1024 * 1024)).toFixed(0);
                showAlert(`File "${file.name}" exceeds maximum size of ${maxMB} MB`, 'danger');
                return false;
            }

            return true;
        });

        if (validFiles.length === 0) return;

        uploadFiles(validFiles);
    }

    /**
     * Upload files to the server
     */
    async function uploadFiles(files) {
        showProgress(true, files.length);

        let successCount = 0;
        let errorCount = 0;

        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const formData = new FormData();
            formData.append('entityType', config.entityType);
            formData.append('entityId', config.entityId);
            formData.append('file', file);

            try {
                const response = await fetch('/api/attachments', {
                    method: 'POST',
                    headers: Auth.authHeaders(),
                    body: formData
                });

                if (response.ok) {
                    successCount++;
                    const attachment = await response.json();
                    addFileItem(attachment);
                } else {
                    errorCount++;
                    const error = await response.json().catch(() => ({ message: 'Upload failed' }));
                    console.error(`Upload failed for ${file.name}:`, error);
                }
            } catch (error) {
                errorCount++;
                console.error(`Upload error for ${file.name}:`, error);
            }

            updateProgress((i + 1) / files.length);
        }

        showProgress(false);

        if (errorCount === 0) {
            showAlert(`Successfully uploaded ${successCount} file(s)`, 'success');
        } else if (successCount > 0) {
            showAlert(`Uploaded ${successCount} file(s) with ${errorCount} error(s)`, 'warning');
        } else {
            showAlert(`Failed to upload ${errorCount} file(s)`, 'danger');
        }

        document.getElementById('fileInput').value = '';
    }

    /**
     * Load and display existing files
     */
    async function loadExistingFiles() {
        try {
            const params = new URLSearchParams({
                entityType: config.entityType,
                entityId: config.entityId
            });

            const response = await fetch(`/api/attachments?${params}`);
            if (!response.ok) throw new Error('Failed to load files');

            const attachments = await response.json();
            const fileItems = document.getElementById('fileItems');
            const noFiles = document.getElementById('noFiles');

            if (attachments.length === 0) {
                fileItems.innerHTML = '';
                noFiles.style.display = 'block';
            } else {
                fileItems.innerHTML = '';
                noFiles.style.display = 'none';
                attachments.forEach(attachment => addFileItem(attachment));
            }
        } catch (error) {
            console.error('Error loading files:', error);
        }
    }

    /**
     * Add a file item to the list
     */
    function addFileItem(attachment) {
        const fileItems = document.getElementById('fileItems');
        const noFiles = document.getElementById('noFiles');

        const icon = FILE_ICONS[attachment.contentType] || '📎';
        const fileSize = formatFileSize(attachment.fileSize);
        const createdDate = new Date(attachment.createdAt).toLocaleDateString();

        const fileItem = document.createElement('div');
        fileItem.className = 'file-item';
        fileItem.id = `file-${attachment.id}`;
        fileItem.innerHTML = `
            <div class="file-info">
                <div class="file-icon">${icon}</div>
                <div class="file-details">
                    <span class="file-name" title="${attachment.fileName}">${escapeHtml(attachment.fileName)}</span>
                    <div class="file-meta">
                        <span class="file-size">${fileSize}</span>
                        <span class="file-date">${createdDate}</span>
                    </div>
                </div>
            </div>
            <div class="file-actions">
                <a href="${attachment.filePath}" class="btn-download" download title="Download file">
                    ⬇️
                </a>
                <button class="btn-delete" data-id="${attachment.id}" title="Delete file">
                    ✕
                </button>
            </div>
        `;

        // Add delete handler
        const deleteBtn = fileItem.querySelector('.btn-delete');
        deleteBtn.addEventListener('click', () => deleteFile(attachment.id, fileItem));

        fileItems.appendChild(fileItem);
        noFiles.style.display = 'none';
    }

    /**
     * Delete a file
     */
    async function deleteFile(fileId, fileItem) {
        if (!confirm('Are you sure you want to delete this file?')) {
            return;
        }

        const deleteBtn = fileItem.querySelector('.btn-delete');
        deleteBtn.disabled = true;
        deleteBtn.textContent = '...';

        try {
            const response = await fetch(`/api/attachments/${fileId}`, {
                method: 'DELETE',
                headers: Auth.authHeaders()
            });

            if (response.ok || response.status === 204) {
                fileItem.remove();
                showAlert('File deleted successfully', 'success');

                // Show "no files" message if list is empty
                const fileItems = document.getElementById('fileItems');
                if (fileItems.children.length === 0) {
                    document.getElementById('noFiles').style.display = 'block';
                }
            } else {
                throw new Error('Delete failed');
            }
        } catch (error) {
            console.error('Error deleting file:', error);
            showAlert('Failed to delete file. You may not have permission.', 'danger');
            deleteBtn.disabled = false;
            deleteBtn.textContent = '✕';
        }
    }

    /**
     * Show/hide progress bar
     */
    function showProgress(show, count = 0) {
        const progress = document.getElementById('uploadProgress');
        const uploadCount = document.getElementById('uploadCount');

        if (show) {
            uploadCount.textContent = count;
            progress.classList.add('show');
        } else {
            progress.classList.remove('show');
        }
    }

    /**
     * Update progress bar
     */
    function updateProgress(percent) {
        const progressBar = document.getElementById('progressBar');
        progressBar.style.width = (percent * 100) + '%';
    }

    /**
     * Show alert message
     */
    function showAlert(message, type) {
        const alert = document.getElementById('uploadAlert');
        alert.textContent = message;
        alert.className = `alert alert-${type}`;
        alert.classList.add('show');

        // Auto-hide after 5 seconds
        setTimeout(() => {
            alert.classList.remove('show');
        }, 5000);
    }

    /**
     * Format file size for display
     */
    function formatFileSize(bytes) {
        if (bytes === 0) return '0 Bytes';

        const k = 1024;
        const sizes = ['Bytes', 'KB', 'MB'];
        const i = Math.floor(Math.log(bytes) / Math.log(k));

        return Math.round(bytes / Math.pow(k, i) * 100) / 100 + ' ' + sizes[i];
    }

    /**
     * Escape HTML to prevent XSS
     */
    function escapeHtml(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, m => map[m]);
    }

    return {
        init: init
    };
})();
