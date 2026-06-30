class SwalUtil{
    constructor(){ }

    confirm(text, onYes, onNo = () => { }){
        Swal.fire({
            text: text,
            icon: "question",
            showCancelButton: !0,
            buttonsStyling: !1,
            confirmButtonText: "Yes",
            cancelButtonText: "No",
            customClass: { confirmButton: "btn btn-primary", cancelButton: "btn btn-active-light" },
        }).then(function (t) {
            t.value ? onYes() : onNo();
        });
    }

    error(text, html = '') {
        if (html == '') {
            Swal.fire({
                text: text,
                icon: "error",
                buttonsStyling: !1,
                confirmButtonText: "Ok",
                customClass: { confirmButton: "btn btn-primary" },
            });
        }
        else {
            Swal.fire({
                title: text,
                html: html,
                icon: 'error',
                confirmButtonText: 'Close'
            });
        }
    }
    info(text, html = '') {
        if (html == '') {
            Swal.fire({
                text: text,
                icon: "info",
                buttonsStyling: !1,
                confirmButtonText: "Ok",
                customClass: { confirmButton: "btn btn-primary" },
            });
        }
        else {
            Swal.fire({
                title: text,
                html: html,
                icon: 'info',
                confirmButtonText: 'Close'
            });
        }
    }

    alert(text, onok){
        Swal.fire({ text: text, icon: "success", buttonsStyling: !1, confirmButtonText: "Ok", customClass: { confirmButton: "btn btn-primary" } })
        .then(()=>{
            if(onok) onok();}
        );
    }

}
