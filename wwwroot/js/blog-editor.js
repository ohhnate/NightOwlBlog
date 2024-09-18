tinymce.init({
    selector: '#editor',
    plugins: 'advlist autolink lists link image charmap print preview hr anchor pagebreak media',
    toolbar: 'undo redo | styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image media',
    menubar: 'file edit view insert format tools table help',
    content_css: '/css/tinymce-night-owl.css',
    skin: 'oxide-dark',
    content_style: 'body { color: #e0e0e0; }',
    setup: function (editor) {
        editor.on('init', function () {
            this.getBody().style.backgroundColor = '#1e1e1e';
        });
        editor.on('change', function () {
            tinymce.triggerSave();
        });
    },
    branding: false,
    promotion: false,
    media_live_embeds: true,
    extended_valid_elements: 'iframe[src|frameborder|style|scrolling|class|width|height|name|align]',
    custom_elements: 'iframe',
    media_url_resolver: function (data, resolve, reject) {
        if (data.url.indexOf('youtube.com') !== -1 || data.url.indexOf('youtu.be') !== -1) {
            var embedHtml = '<iframe width="560" height="315" src="https://www.youtube.com/embed/' + getYoutubeId(data.url) + '" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>';
            resolve({ html: embedHtml });
        } else if (data.url.indexOf('.gif') !== -1) {
            var embedHtml = '<img src="' + data.url + '" alt="GIF" />';
            resolve({ html: embedHtml });
        } else {
            reject({ msg: 'Not a supported media' });
        }
    },
    media_dimensions: false,
});

function getYoutubeId(url) {
    var regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
    var match = url.match(regExp);
    return (match && match[2].length === 11) ? match[2] : null;
}

document.getElementById('blogPostForm').addEventListener('submit', function (e) {
    e.preventDefault();
    console.log('Form submission intercepted');

    var content = tinymce.get('editor').getContent();
    if (!content) {
        alert('Please enter some content for your blog post.');
        return;
    }

    // Ensure the content is set in the textarea
    document.getElementById('editor').value = content;

    console.log('Form data:', new FormData(this));

    fetch(this.action, {
        method: 'POST',
        body: new FormData(this),
        headers: {
            'X-Requested-With': 'XMLHttpRequest'
        }
    }).then(response => {
        if (response.redirected) {
            window.location.href = response.url;
        } else {
            return response.text();
        }
    }).then(html => {
        if (html) {
            document.open();
            document.write(html);
            document.close();
        }
    }).catch(error => {
        console.error('Error:', error);
        alert('An error occurred while submitting the form. Please try again.');
    });
});