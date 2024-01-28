function generateUUID() {
    let date = new Date().getTime();
    let time = ((typeof performance !== 'undefined') && performance.now && (performance.now() * 1000)) || 0;    // Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        let r = Math.random() * 16;                                                                             // Random number between 0 and 16
        if (date > 0) {                                                                                         // Use timestamp until depleted
            r = (date + r) % 16 | 0;
            date = Math.floor(date / 16);
        } else {                                                                                                // Use microseconds since page-load if supported
            r = (time + r) % 16 | 0;
            time = Math.floor(time / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

export { generateUUID };