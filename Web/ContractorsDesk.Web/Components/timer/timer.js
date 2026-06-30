class Timer {
    constructor(displayElement) {
        this.displayElement = displayElement;
        this.totalSeconds = 0; // Tracks total elapsed time in seconds
        this.interval = null; // Holds the interval ID for the timer
    }

    // Start the timer
    start() {
        if (this.interval) return; // Prevent multiple intervals
        this.interval = setInterval(() => {
            this.totalSeconds++;
            this.updateDisplay();
        }, 1000);
    }

    // Pause the timer
    pause() {
        clearInterval(this.interval);
        this.interval = null;
    }

    // Reset the timer
    reset() {
        this.pause();
        this.totalSeconds = 0;
        this.updateDisplay();
    }

    // Update the display in hh:mm:ss format
    updateDisplay() {
        const hours = String(Math.floor(this.totalSeconds / 3600)).padStart(2, '0');
        const minutes = String(Math.floor((this.totalSeconds % 3600) / 60)).padStart(2, '0');
        const seconds = String(this.totalSeconds % 60).padStart(2, '0');
        this.displayElement.textContent = `${hours}:${minutes}:${seconds}`;
    }
}