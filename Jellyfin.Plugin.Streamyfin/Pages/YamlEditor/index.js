const yamlEditor = () => document.getElementById('yaml-editor');
const exampleBtn = () => document.getElementById("example-btn")
const saveBtn = () => document.getElementById("save-btn");

export default function (view, params) {

    // init code here
    view.addEventListener('viewshow', (e) => {
        import(window.ApiClient.getUrl("web/configurationpage?name=shared.js")).then((shared) => {
            shared.setPage("Yaml");
            return shared;
        }).then(async (shared) => {
            // Import monaco after shared resources and wait until its done before continuing
            if (!window.monaco) {
                Dashboard.showLoadingMsg();
                await import(window.ApiClient.getUrl('web/configurationpage?name=monaco-editor.bundle.js'))
            }

            const Page = {
                editor: null,
                yaml: null,
                saveConfig: function (e) {
                    e.preventDefault();
                    shared.setYamlConfig(Page.editor.getModel().getValue())
                    shared.saveConfig()
                },
                loadConfig: function (config) {
                    Dashboard.hideLoadingMsg();
                    const yamlModelUri = monaco.Uri.parse('streamyfin.yaml');

                    Page.editor = monaco.editor.create(yamlEditor(), {
                        automaticLayout: true,
                        language: 'yaml',
                        suggest: {
                            showWords: false
                        },
                        model: monaco.editor.createModel(shared.tools.jsYaml.dump(config), 'yaml', yamlModelUri),
                    });

                    Page.editor.onDidChangeModelContent(function (e) {
                        if (e.eol === '\n' && e.changes[0].text.endsWith(" ")) {
                            // need timeout so it triggers after auto formatting
                            setTimeout(() => {
                                Page.editor.trigger('', 'editor.action.triggerSuggest', {});
                            }, "100");
                        }
                    });

                },
                resetConfig: function () {
                    const example = shared.getDefaultConfig();
                    Page.editor.getModel().setValue(shared.tools.jsYaml.dump(example));
                },
                init: function () {
                    console.log("init");

                    // Yaml Editor
                    monaco.editor.setTheme('vs-dark');
                    
                    
                    Page.yaml = monacoYaml.configureMonacoYaml(monaco, {
                        enableSchemaRequest: true,
                        hover: true,
                        completion: true,
                        validate: true,
                        format: true,
                        titleHidden: true,
                        schemas: [
                            {
                                uri: shared.SCHEMA_URL,
                                fileMatch: ["**/*"]
                            },
                        ],
                    });

                    saveBtn().addEventListener("click", Page.saveConfig);
                    exampleBtn().addEventListener("click", Page.resetConfig);

                    if (shared.getConfig() && Page.editor == null) {
                        Page.loadConfig(shared.getConfig());
                    }

                    shared.setOnConfigUpdatedListener('yaml-editor', (config) => {
                        // only set if editor isn't instantiated 
                        if (Page.editor == null) {
                            console.log("loading")
                            Page.loadConfig(config)
                        } else {
                            Page.editor.getModel().setValue(shared.tools.jsYaml.dump(config))
                        }
                    })
                }
            };

            if (!Page.editor && monaco?.editor?.getModels?.()?.length === 0) {
                Page.init();
            } else {
                console.log("Monaco editor model already exists")
            }

            view.addEventListener('viewhide', function (e) {
                console.log("Hiding")
                Page?.editor?.dispose()
                Page?.yaml?.dispose()
                Page.editor = undefined;
                Page.yaml = undefined;
                monaco?.editor?.getModels?.()?.forEach(model => model.dispose())
                monaco?.editor?.getEditors?.()?.forEach(editor => editor.dispose());
            });
        })
    });
}