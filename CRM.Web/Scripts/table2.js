// Expandable Data Table
$('.data-expands').each(function () {
	$(this).click(function () {
		$(this).toggleClass('row-active');
		$(this).parent().find('.expandable').toggleClass('row-open');
		$(this).parent().find('.row-toggle').toggleClass('row-toggle-twist');
	});
});



