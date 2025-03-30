$(document).on("change", "#CategoryId.get-subcategories", function (e) {
    var categoryId = $(this).val();
    var url = "/Data/GetTicketSubCategories/" + categoryId;

    console.log("Mengambil data dari:", url);

    $.ajax({
        url: url,
        dataType: "json",
        beforeSend: function () {
            console.log("Mengirim request ke:", url);
            $("#SubCategoryId").html("<option value>Loading...</option>"); // Indikasi loading
        },
        success: function (json) {
            console.log("Data diterima:", json);

            // Kosongkan dropdown sebelum menambah data baru
            $("#SubCategoryId").html("");
            $("#SubCategoryId").append("<option value=''>-- Select Sub-Category --</option>");

            if (json.length > 0) {
                $(json).each(function () {
                    $("#SubCategoryId").append(
                        $("<option></option>").val(this.id).html(this.name)
                    );
                });
            } else {
                $("#SubCategoryId").append("<option value=''>No subcategories available</option>");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error AJAX:", xhr.status, error);
        }
    });
});
