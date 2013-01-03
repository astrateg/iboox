$(".delete-account-button").click(function () {
	var userName = $(this).attr("id");
	$.post(
		"/Account/DeleteAccount",
		{ id: userName },
		function (data) {
			$("#account-" + data.id).slideUp();
		},
		"json"
	);
	return false;
});