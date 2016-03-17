$(document).ready(function () {
    var max_fields = 100; //maximum input boxes allowed
    var wrapper = $(".inputFieldsWrap"); //Fields wrapper
    var add_button = $("#addFieldButton"); //Add button ID

    var x = 0; //initlal text box count
    $(add_button).click(function (e) { //on add input button click
        e.preventDefault();
        if (x < max_fields) { //max input box allowed
            x++; //text box increment
            $(wrapper).append('<div class="row"><div class="col-md-11"><input type="text" class="form-control marginBottom" required=""/></div><a href="#" class="removeField"><span class="glyphicon glyphicon-remove blackGlyph" aria-hidden="true"></span></a></div>'); //add input box
        }
    });

    $(wrapper).on("click", ".removeField", function (e) { //user click on remove text
        e.preventDefault(); $(this).parent('div').remove(); x--;
    });

    //submit form
    $("#headersForm").submit(function () {
        $("#alertsWrapper").children().remove();
        var model = new Object();
        var inputs = $("div > div > input", ".inputFieldsWrap");
        var inputName = $("#nameInput");
        model.Name = inputName.val();
        model.Headers = new Array();
        for (var i = 0; i < inputs.length; ++i) {
            model.Headers.push($(inputs[i]).val());
        }
        var url = "AddLogType";

        $.ajax({
            type: "POST",
            url: url,
            data: model,
            success: function (data) {
                if (data.HasError) {
                    $("#alertsWrapper").append('<div class="alert alert-danger"> <a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>Błąd!</strong>' + data.ErrorMessage + '</div>')
                }
                else if (data.Success) {
                    $("#alertsWrapper").append('<div class="alert alert-success"><a href="#" class="close" data-dismiss="alert" aria-label="close">&times;</a><strong>Sukces!</strong>' + data.SuccessMessage + '</div>');
                    $('.inputFieldsWrap').children().remove();
                    x = 0;
                    inputName.val('');
                }
            }
        });

        return false; // avoid to execute the actual submit of the form.
    });
});