// GPU Nexus — Site JS

document.addEventListener('DOMContentLoaded', function () {

    // ── VRAM checkbox styling ──────────────────────────────
    document.querySelectorAll('.vram-check-label').forEach(label => {
        const cb = label.querySelector('input[type="checkbox"]');
        function sync() {
            label.classList.toggle('checked', cb.checked);
        }
        cb.addEventListener('change', sync);
        sync();
    });

    // ── Brand pill sync with sidebar checkboxes ────────────
    const brandPills = document.querySelectorAll('.brand-pill');
    brandPills.forEach(pill => {
        const brand = pill.dataset.brand;
        const sidebarCb = document.querySelector(`#check-${brand}`);

        function syncPill() {
            const active = sidebarCb ? sidebarCb.checked : pill.classList.contains(`active-${brand}`);
            pill.classList.toggle(`active-${brand}`, active);
        }

        pill.addEventListener('click', function () {
            if (sidebarCb) {
                sidebarCb.checked = !sidebarCb.checked;
                // Update sidebar brand-check-label check-box visual
                const box = document.querySelector(`#check-${brand}-box`);
                if (box) box.textContent = sidebarCb.checked ? '✓' : '';
            }
            syncPill();
        });

        if (sidebarCb) {
            sidebarCb.addEventListener('change', syncPill);
        }
        syncPill();
    });

    // ── Brand checkbox visual update ──────────────────────
    document.querySelectorAll('.brand-check-label').forEach(label => {
        const cb = label.querySelector('input[type="checkbox"]');
        const box = label.querySelector('.brand-check-box');
        function syncBox() {
            box.textContent = cb.checked ? '✓' : '';
        }
        cb.addEventListener('change', syncBox);
        syncBox();
    });

    // ── Filter toggle visual ───────────────────────────────
    document.querySelectorAll('.filter-toggle').forEach(label => {
        const cb = label.querySelector('input[type="checkbox"]');
        const dot = label.querySelector('.toggle-dot');
        function syncDot() {
            if (dot) dot.textContent = cb.checked ? '✓' : '';
        }
        if (cb) { cb.addEventListener('change', syncDot); syncDot(); }
    });

    // ── Sort select auto-submit ────────────────────────────
    const sortSelect = document.getElementById('sort-select');
    if (sortSelect) {
        sortSelect.addEventListener('change', function () {
            document.getElementById('filter-form').submit();
        });
    }

    // ── Admin: confirm delete ──────────────────────────────
    document.querySelectorAll('.delete-form').forEach(form => {
        form.addEventListener('submit', function (e) {
            const name = this.dataset.gpuName || 'this GPU';
            if (!confirm(`Remove "${name}" from inventory? This cannot be undone.`)) {
                e.preventDefault();
            }
        });
    });

    // ── Admin: sale price toggle ───────────────────────────
    const isOnSaleCb = document.getElementById('IsOnSale');
    const salePriceGroup = document.getElementById('sale-price-group');
    if (isOnSaleCb && salePriceGroup) {
        function toggleSalePrice() {
            salePriceGroup.style.display = isOnSaleCb.checked ? 'flex' : 'none';
        }
        isOnSaleCb.addEventListener('change', toggleSalePrice);
        toggleSalePrice();
    }

    // ── Add to Cart button handler ─────────────────────────
    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.cart-btn').forEach(btn => {
            btn.addEventListener('click', async function (e) {
                e.preventDefault();

                const card = this.closest('.gpu-card');
                if (!card) return;

                const gpuId = card.dataset.gpuId;
                if (!gpuId) {
                    alert('Error: Could not identify GPU. Please refresh the page.');
                    return;
                }

                const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value || '';

                const formData = new FormData();
                formData.append('gpuId', gpuId);
                formData.append('quantity', '1');
                formData.append('__RequestVerificationToken', token);

                try {
                    const response = await fetch('/Cart/AddToCart', {
                        method: 'POST',
                        body: formData
                    });

                    if (!response.ok) {
                        alert('Server error: ' + response.status);
                        return;
                    }

                    const result = await response.json();
                    alert(result.message);

                    if (result.success && result.cartCount !== undefined) {
                        const badge = document.querySelector('.cart-count');
                        if (badge) badge.textContent = result.cartCount;
                    }

                } catch (err) {
                    console.error(err);
                    alert('Something went wrong. Please try again.');
                }
            });
        });
    });

    // ── GPU card hover sparkle ─────────────────────────────
    document.querySelectorAll('.gpu-card').forEach(card => {
        card.addEventListener('mouseenter', function () {
            this.style.transition = 'all 0.18s ease';
        });
    });

    // ── Animate deal cards on load ─────────────────────────
    const dealCards = document.querySelectorAll('.deal-card');
    dealCards.forEach((card, i) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(16px)';
        setTimeout(() => {
            card.style.transition = 'opacity 0.4s ease, transform 0.4s ease';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 80 + i * 80);
    });

    const gpuCards = document.querySelectorAll('.gpu-card');
    gpuCards.forEach((card, i) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(12px)';
        setTimeout(() => {
            card.style.transition = 'opacity 0.35s ease, transform 0.35s ease, background var(--transition), border-color var(--transition), box-shadow var(--transition)';
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 50 + i * 40);
    });
});
