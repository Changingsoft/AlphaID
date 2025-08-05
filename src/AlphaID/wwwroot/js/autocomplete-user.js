(function () {
    function initAutocomplete($input) {
        var sourceUrl = $input.data('autocomplete-source');
        if (!sourceUrl) return;

        $input.autocomplete({
            source: sourceUrl,
            minLength: 2,
            appendTo: $input.data('autocomplete-appendto') || null,
            select: function (event, ui) {
                $input.val(ui.item.name);
                $input.data('autocomplete-target')
                    ? $($input.data('autocomplete-target')).val(ui.item.userId)
                    : $input.val(ui.item.userId);
                return false;
            }
        })
        .autocomplete("instance")._renderItem = function (ul, item) {
            var div = $("<div>").attr("class", "row")
                .append(
                    $("<div>").attr("class", "col-auto")
                        .append(
                            $("<img>").attr("src", item.avatarUrl).attr("class", "avatar")
                        )
                )
                .append(
                    $("<div>").attr("class", "col text-truncate")
                        .append(
                            $("<p>").append(item.name)
                        )
                        .append(
                            $("<p>").attr("class", "text-secondary")
                                .append(item.userName)
                        )
                );
            var li = $("<li>").append(div);
            return li.appendTo(ul);
        };
    }

    $(function () {
        $("input[data-autocomplete='user']").each(function () {
            initAutocomplete($(this));
        });
    });
})();