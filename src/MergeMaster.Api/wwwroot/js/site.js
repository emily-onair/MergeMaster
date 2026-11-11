// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// site.js
document.querySelectorAll('form[data-disable-on-submit]').forEach(form => {
    form.addEventListener('submit', () => {
        form.querySelector('button[type=submit]').disabled = true;
        form.querySelector('[data-loading-spin]')?.classList.add('infinite-spin');
    });
});

const modalEl = document.getElementById('modal');
if (modalEl) {

    let loadController = null;

    modalEl.addEventListener('show.bs.modal', async(e) => {
        console.log("modal opens");
        const trigger = e.relatedTarget;
        console.log(trigger)
        if (!trigger?.dataset.modalUrl) return;
        console.log("after trigger")

        loadController?.abort();
        const controller = new AbortController();
        loadController = controller;

        const title = modalEl.querySelector("#modalTitle");
        console.log(title);
        const body = modalEl.querySelector(".modal-body");
        console.log(body);
        title.textContent = trigger.dataset.modalTitle ?? "";
        body.innerHTML = `
            <div class="text-center p-3">
                <div class="spinner-border spinner-border-sm"></div>
            </div>
            `;

        try {
            const res = await fetch(trigger.dataset.modalUrl, 
                { signal: controller.signal });
            if (!res.ok) throw new Error(res.status);
            body.innerHTML = await res.text();
        } catch(error) {
            if (error.name === "AbortError") return;
            body.innerHTML = `
                <p class="text-danger mb-0">Could not load flags.</p>
            `;
        }
    })

    modalEl.addEventListener('hidden.bs.modal', () => loadController?.abort());
}