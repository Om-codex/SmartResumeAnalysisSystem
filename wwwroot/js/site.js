const themeToggle = document.getElementById('themeToggle');
const savedTheme = localStorage.getItem('theme');

if (savedTheme === 'dark') {
    document.body.classList.add('dark');
}

function updateThemeButton() {
    if (!themeToggle) return;
    themeToggle.textContent = document.body.classList.contains('dark') ? 'Light Mode' : 'Dark Mode';
}

updateThemeButton();

themeToggle?.addEventListener('click', () => {
    document.body.classList.toggle('dark');
    localStorage.setItem('theme', document.body.classList.contains('dark') ? 'dark' : 'light');
    updateThemeButton();
});

const templates = {
    Education: index => `
        <div class="item-row">
            <input name="Education[${index}].Degree" placeholder="Degree / Course" />
            <input name="Education[${index}].Institution" placeholder="Institution" />
            <input name="Education[${index}].PassingYear" placeholder="Passing Year" />
            <input name="Education[${index}].Score" placeholder="CGPA / Percentage" />
        </div>`,
    Skills: index => `
        <div class="item-row skill-row">
            <input name="Skills[${index}].SkillName" placeholder="Skill name" />
            <select name="Skills[${index}].SkillType">
                <option>Technical</option>
                <option>Tool</option>
                <option>Soft Skill</option>
            </select>
        </div>`,
    Projects: index => `
        <div class="item-row project-row">
            <input name="Projects[${index}].Title" placeholder="Project title" />
            <textarea name="Projects[${index}].Description" rows="2" placeholder="Project description"></textarea>
            <input name="Projects[${index}].TechnologiesUsed" placeholder="Technologies used" />
        </div>`,
    Experience: index => `
        <div class="item-row project-row">
            <input name="Experience[${index}].CompanyName" placeholder="Company / Organization" />
            <input name="Experience[${index}].Role" placeholder="Role" />
            <input name="Experience[${index}].Duration" placeholder="Duration" />
            <textarea name="Experience[${index}].Responsibilities" rows="2" placeholder="Responsibilities"></textarea>
        </div>`
};

document.querySelectorAll('[data-add]').forEach(button => {
    button.addEventListener('click', () => {
        const section = button.dataset.add;
        const container = document.getElementById(`${section}Items`);
        if (!container || !templates[section]) return;

        const index = container.querySelectorAll('.item-row').length;
        container.insertAdjacentHTML('beforeend', templates[section](index));
    });
});

if ('serviceWorker' in navigator) {
    window.addEventListener('load', () => {
        navigator.serviceWorker.register('/service-worker.js').catch(() => {
            // The app still works normally if the browser blocks service workers on localhost.
        });
    });
}
