// Write your Javascript code.
function formatRepo(repo) {
    if (repo.loading) return repo.text;
    return repo.value;
}

function formatRepoSelection(repo) {
    return repo.value || repo.text;
}

$("#account").select2({
    placeholder: '',
    allowClear: true,
    ajax: {
        url: "/Search/Account",
        dataType: 'json',
        delay: 250,
        data: function (params) {
            return {
                query: params.term
            };
        },
        processResults: function (data, params) {
            // parse the results into the format expected by Select2
            // since we are using custom formatting functions we do not need to
            // alter the remote JSON data, except to indicate that infinite
            // scrolling can be used
            params.page = params.page || 1;

            return {
                results: data,
                pagination: {
                    more: (params.page * 30) < data.total_count
                }
            };
        },
        cache: true
    },
    escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
    minimumInputLength: 3,
    templateResult: formatRepo, // omitted for brevity, see the source of this page
    templateSelection: formatRepoSelection // omitted for brevity, see the source of this page
});

$("#project").select2({
    placeholder: '',
    allowClear: true,
    ajax: {
        url: "/Search/Project",
        allowClear: true,
        dataType: 'json',
        delay: 250,
        data: function (params) {
            return {
                query: params.term
            };
        },
        processResults: function (data, params) {
            // parse the results into the format expected by Select2
            // since we are using custom formatting functions we do not need to
            // alter the remote JSON data, except to indicate that infinite
            // scrolling can be used
            params.page = params.page || 1;

            return {
                results: data,
                pagination: {
                    more: (params.page * 30) < data.total_count
                }
            };
        },
        cache: true
    },
    escapeMarkup: function (markup) { return markup; }, // let our custom formatter work
    minimumInputLength: 3,
    templateResult: formatRepo, // omitted for brevity, see the source of this page
    templateSelection: formatRepoSelection // omitted for brevity, see the source of this page
});