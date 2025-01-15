export default function (view) {
    const Streamyfin = {
        pluginId: "1e9e5d38-6e67-4615-8719-e98a5c34f004",
        btnSave: document.querySelector("#saveConfig"),
        editor: null,
        saveConfig: function (e) {
            e.preventDefault();
            Dashboard.showLoadingMsg();
            const config = {
                Value: Streamyfin.editor.getModel().getValue()
            };

            const url = window.ApiClient.getUrl('streamyfin/config/yaml');
            const data = JSON.stringify(config);

            window.ApiClient.ajax({ type: 'POST', url, data, contentType: 'application/json' })
                .then(function (response) {
                        response.json().then(res => {
                            if (res.Error == true) {
                                Dashboard.hideLoadingMsg();
                                Dashboard.alert(res.Message);
                            } else {
                                Dashboard.processPluginConfigurationUpdateResult();
                            }
                        })
                    }

                )
                .catch(function (error) {
                    //alert(error);
                    console.error(error);
                })
                .finally(function () {
                    Dashboard.hideLoadingMsg();
                });

        },
        loadConfig: function () {
            Dashboard.showLoadingMsg();
            const url = window.ApiClient.getUrl('streamyfin/config/yaml');
            window.ApiClient.ajax({ type: 'GET', url, contentType: 'application/json' })
                .then(function (response) {
                    response.json().then(res => {
                        const yamlModelUri = monaco.Uri.parse('streamyfin.yaml');
                        Streamyfin.editor = monaco.editor.create(document.getElementById('yamleditor'), {
                            automaticLayout: true,
                            language: 'yaml',
                            quickSuggestions: {
                                other: true,
                                comments: false,
                                strings: true
                            },
                            acceptSuggestionOnEnter: 'on',
                            model: monaco.editor.createModel(res.Value, 'yaml', yamlModelUri),
                        });
                    })
                })
                .catch(function (error) {
                    console.error(error);
                })
                .finally(function () {
                    Dashboard.hideLoadingMsg();
                });
        },
        init: function () {
            monaco.editor.setTheme('vs-dark');
            const monaco_yaml = monacoYaml.configureMonacoYaml(monaco, {
                enableSchemaRequest: true,
                hover: true,
                completion: true,
                validate: true,
                format: true,
                schemas: [
                    {
                        uri: location.origin + '/streamyfin/config/schema',
                        fileMatch: ["*"],
                    },
                ],
            });
            console.log("init");
            Streamyfin.loadConfig();
            Streamyfin.btnSave.addEventListener("click", Streamyfin.saveConfig);
        }
    }

    view.addEventListener("viewshow", function (e) {
        waitForScript();
    });

    function waitForScript() {
        if (typeof monaco === "undefined") {
            setTimeout(waitForScript, 50);
        } else {
            Streamyfin.init();
        }
    }
}


// waitForScript();
