// wwwroot/js/exam-timer.js
class ExamTimer {
    constructor(durationMinutes, startTime) {
        this.endTime = new Date(startTime);
        this.endTime.setMinutes(this.endTime.getMinutes() + durationMinutes);
        this.timerElement = document.getElementById('exam-timer');
        this.warningShown = false;
        
        this.start();
    }
    
    start() {
        this.updateTimer();
        this.interval = setInterval(() => this.updateTimer(), 1000);
    }
    
    updateTimer() {
        const now = new Date();
        const remaining = this.endTime - now;
        
        if (remaining <= 0) {
            this.timeUp();
            return;
        }
        
        const minutes = Math.floor(remaining / 60000);
        const seconds = Math.floor((remaining % 60000) / 1000);
        
        this.timerElement.textContent = 
            `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
        
        // Warning at 5 minutes
        if (minutes < 5 && !this.warningShown) {
            this.showWarning();
            this.warningShown = true;
        }
        
        // Change color when time is running out
        if (minutes < 5) {
            this.timerElement.classList.add('text-danger');
        }
    }
    
    showWarning() {
        alert('Warning: Only 5 minutes remaining!');
    }
    
    timeUp() {
        clearInterval(this.interval);
        this.timerElement.textContent = '00:00';
        alert('Time is up! Submitting exam automatically...');
        document.getElementById('exam-form').submit();
    }
}

// Auto-save answers every 30 seconds
let autoSaveInterval = setInterval(() => {
    const formData = new FormData(document.getElementById('exam-form'));
    
    fetch('/Student/Exams/AutoSave', {
        method: 'POST',
        body: formData
    });
}, 30000);