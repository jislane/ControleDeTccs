showInPopUp = (url, title) => {
    $.ajax({
        type:"GET",
        url: url,
        success: function (res) {
            $("#janela-modal .modal-body").html(res);
            $("#janela-modal .modal-title").html(title);
            $("#janela-modal").modal('show');
        }
    })
}