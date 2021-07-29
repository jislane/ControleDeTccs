(function () {
    "use strict"
    var language = {
        noResults: function () {
            return "Sem resultados";
        },
        inputTooShort: function (args) {
            return "Insira " + args.minimum + " ou mais caracteres";
        },
        searching: function () {
            return "Buscando...";
        },
        errorLoading: function () {
            return "Erro ao carregar resultados";
        },
    };

    function orientadorSelect() {

        $(document).ready(function () {

            $('#select-orientador').select2();
            $("#select-orientador").select2({
                ajax: {
                    url: "/tccs/getOrientadores",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        var idc = document.querySelector("#select-campus").value;
                        return {
                            idCampus: idc,
                            nome: params.term
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data,
                        };
                    },
                },
                placeholder: 'Busca por Orientador',
                minimumInputLength: 1,
                language: language,
                escapeMarkup: function (markup) {
                    return markup;
                },
                templateResult: formatRepo,
                templateSelection: formatRepoSelection
            });

            function formatRepo(repo) {
                if (repo.loading) {
                    return repo.text;
                }
                var $container = $(
                    "<div class='select2-result-orientador clearfix'>" +
                    "<div class='select2-result-nome'></div>" +
                    "</div>"
                );

                $container.find(".select2-result-nome").text(repo.nome);
                return $container;
            }
            function formatRepoSelection(repo) {
                return repo.nome || repo.text;
            }
        });
    }

    function discenteSelect() {

        $(document).ready(function () {

            $('#select-discente').select2();
            $("#select-discente").select2({
                ajax: {
                    url: "/tccs/getDiscentes",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        //var idc = document.querySelector("#select-campus").value;
                        var idcurso = document.querySelector("#select-curso").value;
                        return {
                            idCurso: idcurso,
                            nome: params.term
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data,
                        };
                    },
                },
                placeholder: 'Busca por Discente',
                minimumInputLength: 1,
                language: language,
                escapeMarkup: function (markup) {
                    return markup;
                },
                templateResult: formatRepo,
                templateSelection: formatRepoSelection
            });

            function formatRepo(repo) {
                if (repo.loading) {
                    return repo.text;
                }
                var $container = $(
                    "<div class='select2-result-orientador clearfix'>" +
                    "<div class='select2-result-nome'></div>" +
                    "</div>"
                );

                $container.find(".select2-result-nome").text(repo.nome);
                return $container;
            }
            function formatRepoSelection(repo) {
                return repo.nome || repo.text;
            }
        });
    }

    function CursosSelect() {

        $(document).ready(function () {

            $('#select-curso').select2();
            $("#select-curso").select2({
                ajax: {
                    url: "/tccs/getCursos",
                    dataType: 'json',
                    delay: 250,
                    data: function (params) {
                        var idc = document.querySelector("#select-campus").value;
                        
                        return {
                            idCampus: idc,
                            nome: params.term
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data,
                        };
                    },
                },
                placeholder: 'Busca por Curso',
                minimumInputLength: 1,
                language: language,
                escapeMarkup: function (markup) {
                    return markup;
                },
                templateResult: formatRepo,
                templateSelection: formatRepoSelection
            });

            function formatRepo(repo) {
                if (repo.loading) {
                    return repo.text;
                }
                var $container = $(
                    "<div class='select2-result-orientador clearfix'>" +
                    "<div class='select2-result-nome'></div>" +
                    "</div>"
                );

                $container.find(".select2-result-nome").text(repo.nome);
                return $container;
            }
            function formatRepoSelection(repo) {
                return repo.nome || repo.text;
            }
        });
    }

    CursosSelect();
    orientadorSelect();
    discenteSelect();

})();

