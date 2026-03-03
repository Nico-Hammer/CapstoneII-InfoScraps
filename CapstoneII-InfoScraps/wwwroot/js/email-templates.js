// ============================================================================
// Email Templates Page - Client-side logic
// ============================================================================

// Fallback values used when no lead is selected or lead data is missing.
var VARIABLE_FALLBACKS = {
    'first_name':                'Business Owner',
    'business_name':             'your business',
    'sender_name':               'Your Name',
    'specific_problem':          'improving your online presence',
    'personalized_observation':  'a few areas with potential for growth',
    'industry':                  'your industry',
    'industry_or_location':      'local',
    'specific_outcome':          'increasing lead generation and online visibility',
    'specific_result':           'a 30% increase in qualified leads'
};

// State populated by initEmailTemplates()
var _leads = [];
var _senderName = '';
var _createUrl = '';
var _editUrl = '';
var _currentTemplate = null;

// ----------------------------------------------------------------------------
// Initialisation — called from the Razor view with server-side data
// ----------------------------------------------------------------------------
function initEmailTemplates(leads, senderName, createUrl, editUrl) {
    _leads = leads || [];
    _senderName = senderName || '';
    _createUrl = createUrl || '';
    _editUrl = editUrl || '';

    // Override the sender_name fallback with the actual logged-in username
    if (_senderName) {
        VARIABLE_FALLBACKS['sender_name'] = _senderName;
    }
}

// ============================================================================
// Create / Edit Modal
// ============================================================================
function openCreateModal() {
    document.getElementById('templateModalTitle').textContent = 'Create Template';
    document.getElementById('templateForm').action = _createUrl;
    document.getElementById('templateId').value = '';
    document.getElementById('templateName').value = '';
    document.getElementById('templateSubject').value = '';
    document.getElementById('templateBody').value = '';
    new bootstrap.Modal(document.getElementById('templateModal')).show();
}

function editTemplate(id, data) {
    document.getElementById('templateModalTitle').textContent = 'Edit Template';
    document.getElementById('templateForm').action = _editUrl;
    document.getElementById('templateId').value = id;
    document.getElementById('templateName').value = data.Name;
    document.getElementById('templateSubject').value = data.Subject;
    document.getElementById('templateBody').value = data.Body;
    new bootstrap.Modal(document.getElementById('templateModal')).show();
}

function cloneTemplate(data) {
    document.getElementById('templateModalTitle').textContent = 'Edit Default Template (saves as custom copy)';
    document.getElementById('templateForm').action = _createUrl;
    document.getElementById('templateId').value = '';
    document.getElementById('templateName').value = data.Name;
    document.getElementById('templateSubject').value = data.Subject;
    document.getElementById('templateBody').value = data.Body;
    new bootstrap.Modal(document.getElementById('templateModal')).show();
}

// ============================================================================
// Delete Modal
// ============================================================================
function confirmDelete(id, name) {
    document.getElementById('deleteTemplateId').value = id;
    document.getElementById('deleteTemplateName').textContent = name;
    new bootstrap.Modal(document.getElementById('deleteModal')).show();
}

// ============================================================================
// Use Template / Generate Email
// ============================================================================
function useTemplate(data) {
    _currentTemplate = data;
    document.getElementById('useTemplateName').textContent = data.Name;

    // Populate lead dropdown
    var select = document.getElementById('leadSelect');
    select.innerHTML = '<option value="">-- No lead selected (use fallback values) --</option>';
    _leads.forEach(function (lead) {
        var label = [lead.name, lead.email, lead.website].filter(Boolean).join(' \u2014 ');
        var opt = document.createElement('option');
        opt.value = lead.Id;
        opt.textContent = label;
        select.appendChild(opt);
    });

    select.onchange = function () {
        populateVariables(data.Variables, this.value);
    };

    // Initial population with fallbacks
    populateVariables(data.Variables, '');
    new bootstrap.Modal(document.getElementById('useModal')).show();
}

// ----------------------------------------------------------------------------
// Variable mapping
// ----------------------------------------------------------------------------
function populateVariables(variables, leadId) {
    var container = document.getElementById('variableFields');
    container.innerHTML = '';

    var lead = leadId ? _leads.find(function (l) { return l.Id == leadId; }) : null;

    variables.forEach(function (varName) {
        var value = VARIABLE_FALLBACKS[varName] || '';

        // Auto-map from lead data when a lead is selected
        if (lead) {
            if (varName === 'first_name' && lead.name) {
                value = lead.name.split(' ')[0];
            } else if (varName === 'business_name' && lead.website) {
                value = extractBusinessName(lead.website);
            }
        }

        // sender_name always defaults to the logged-in user
        if (varName === 'sender_name' && _senderName) {
            value = _senderName;
        }

        var item = document.createElement('div');
        item.className = 'is-var-item';

        var label = document.createElement('label');
        label.className = 'is-var-label';
        label.setAttribute('for', 'var_' + varName);
        label.textContent = '{{' + varName + '}}';

        var input = document.createElement('input');
        input.type = 'text';
        input.className = 'is-form-input is-var-input';
        input.id = 'var_' + varName;
        input.dataset.var = varName;
        input.value = value;

        var fallback = document.createElement('span');
        fallback.className = 'is-var-fallback';
        fallback.textContent = 'Default: ' + (VARIABLE_FALLBACKS[varName] || '(none)');

        item.appendChild(label);
        item.appendChild(input);
        item.appendChild(fallback);
        container.appendChild(item);
    });

    // Live preview on every keystroke
    container.querySelectorAll('input').forEach(function (input) {
        input.addEventListener('input', updatePreview);
    });

    updatePreview();
}

// ----------------------------------------------------------------------------
// Live email preview
// ----------------------------------------------------------------------------
function updatePreview() {
    if (!_currentTemplate) return;

    var lead = getSelectedLead();
    var toEmail = lead ? lead.email : '(no lead selected)';
    var subject = _currentTemplate.Subject;
    var body = _currentTemplate.Body;

    document.querySelectorAll('#variableFields input').forEach(function (input) {
        var varName = input.dataset.var;
        var regex = new RegExp('\\{\\{' + varName + '\\}\\}', 'g');
        subject = subject.replace(regex, input.value);
        body = body.replace(regex, input.value);
    });

    document.getElementById('previewTo').textContent = toEmail;
    document.getElementById('previewSubject').textContent = subject;
    document.getElementById('previewBody').textContent = body;
}

// ----------------------------------------------------------------------------
// Generate mailto link and open default email client
// ----------------------------------------------------------------------------
function generateEmail() {
    var lead = getSelectedLead();
    var toEmail = lead ? lead.email : '';
    var subject = _currentTemplate.Subject;
    var body = _currentTemplate.Body;

    document.querySelectorAll('#variableFields input').forEach(function (input) {
        var varName = input.dataset.var;
        var regex = new RegExp('\\{\\{' + varName + '\\}\\}', 'g');
        subject = subject.replace(regex, input.value);
        body = body.replace(regex, input.value);
    });

    var mailto = 'mailto:' + encodeURIComponent(toEmail)
        + '?subject=' + encodeURIComponent(subject)
        + '&body=' + encodeURIComponent(body);

    window.location.href = mailto;
}

// ============================================================================
// Helpers
// ============================================================================
function getSelectedLead() {
    var select = document.getElementById('leadSelect');
    if (!select.value) return null;
    return _leads.find(function (l) { return l.Id == select.value; });
}

function extractBusinessName(url) {
    try {
        var hostname = url;
        if (hostname.indexOf('://') === -1) hostname = 'https://' + hostname;
        hostname = new URL(hostname).hostname;
        hostname = hostname.replace(/^www\./, '');
        var parts = hostname.split('.');
        if (parts.length >= 2) {
            return parts[0].charAt(0).toUpperCase() + parts[0].slice(1);
        }
        return hostname;
    } catch (e) {
        return url;
    }
}
