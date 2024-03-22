$(() => {
    $("#like-button").on('click', function () {
        const id = $('#image-id').val()
        $.post(`/home/incrementlikes?id=${id}`, function () {
            getLikesForImage(id)
        });
        $(this).prop('disabled', true)
    })

    function getLikesForImage(id) {
        $.get(`/home/GetLikesById?id=${id}`, function (likes) {
            $("#likes-count").text(likes)
        })
    }
    setInterval(function () {
        const id = $('#image-id').val()
        getLikesForImage(id)
    }, 2000);
});
