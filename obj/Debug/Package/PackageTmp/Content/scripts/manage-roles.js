$(function () {
	$(".delete-role-button").click(function () {
		var roleId = $(this).attr("id");
		$.post(
			"/Account/DeleteRole", { "id": roleId },
			function (data) {
				$("#role-" + data.id).fadeOut();
				$("#TotalRoles").text(data.TotalRoles);
			},
			"json"
		);
		return false;
	});
});