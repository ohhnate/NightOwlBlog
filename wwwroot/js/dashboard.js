$(document).ready(function () {
    $('#dashboardTabs button').on('click', function (e) {
        e.preventDefault();
        $(this).tab('show');
    });
});