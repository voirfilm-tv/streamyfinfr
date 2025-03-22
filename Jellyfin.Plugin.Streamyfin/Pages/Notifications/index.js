const saveBtn = document.getElementById('save-notification-btn');

const getValues = () => ({
    notifications: Array.from(document.querySelectorAll('[data-key-name][data-prop-name]')).reduce((acc, el) => {
        if (el.offsetParent === null) return acc;
        
        const notification = el.getAttribute('data-key-name');
        const property = el.getAttribute('data-prop-name');
        
        
        console.log("Notification", notification, el.offsetParent)

        const value = window.Streamyfin.shared.getElValue(el);
        acc[notification] = acc[notification] ?? {}

        if (value != null) {
            acc[notification][property] = value;
        }
        else delete acc[notification]

        return acc
    }, {})
})

// region helpers
const updateNotificationConfig = (name, config, valueName, value) => ({
    ...(config ?? {}),
    notifications: {
        ...(config?.notifications ?? {}),
        [name]: {
            ...(config?.notifications?.[name] ?? {}),
            [valueName]: value,
        }
    }
})
// endregion helpers

export default function (view, params) {

    // init code here
    view.addEventListener('viewshow', (e) => {
        import("/web/configurationpage?name=shared.js").then((shared) => {
            shared.setPage("Notifications");
            
            document.getElementById("notification-endpoint").innerText = shared.NOTIFICATION_URL

            shared.setDomValues(document, shared.getConfig()?.notifications)
            shared.setOnConfigUpdatedListener('notifications', (config) => {
                console.log("updating dom for notifications")

                const {notifications} = config;
                shared.setDomValues(document, notifications);
            })

            document.querySelectorAll('[data-key-name][data-prop-name]').forEach(el => {
                shared.keyedEventListener(el, 'change', function () {
                    shared.setConfig(updateNotificationConfig(
                        el.getAttribute('data-key-name'),
                        shared.getConfig(),
                        el.getAttribute('data-prop-name'),
                        shared.getElValue(el)
                    ));
                })
            })

            shared.keyedEventListener(saveBtn, 'click', function (e) {
                e.preventDefault();
                const config = shared.getConfig();

                shared.setConfig({
                    ...config,
                    ...getValues()
                });
                shared.saveConfig()
            })
        })
    });
}