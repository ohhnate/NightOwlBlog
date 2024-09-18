$(document).ready(function () {
    // Toggle reply form
    $('.reply-btn').click(function () {
        var commentId = $(this).data('comment-id');
        $('#reply-form-' + commentId).toggleClass('d-none');
    });

    // Edit comment
    $('.edit-comment-btn').click(function () {
        var commentId = $(this).data('comment-id');
        var commentElement = $('#comment-' + commentId);
        var commentContent = commentElement.find('p').text();
        var editForm = `
            <form asp-action="EditComment" method="post">
                <input type="hidden" name="id" value="${commentId}" />
                <div class="form-group">
                    <textarea name="content" class="form-control" rows="3" required>${commentContent}</textarea>
                </div>
                <button type="submit" class="btn btn-primary mt-2">Save Changes</button>
                <button type="button" class="btn btn-secondary mt-2 cancel-edit-btn">Cancel</button>
            </form>
        `;
        commentElement.find('p').replaceWith(editForm);
        $(this).hide();
    });

    // Cancel edit
    $(document).on('click', '.cancel-edit-btn', function () {
        var form = $(this).closest('form');
        var commentId = form.find('input[name="id"]').val();
        var commentContent = form.find('textarea[name="content"]').val();
        form.replaceWith('<p>' + commentContent + '</p>');
        $('#comment-' + commentId).find('.edit-comment-btn').show();
    });

    // Toggle my comments
    $('#toggleMyComments').click(function () {
        $('.comment').toggle();
        $('.my-comment').toggle();
        $(this).text(function (i, text) {
            return text === "Show My Comments" ? "Show All Comments" : "Show My Comments";
        });
    });
});