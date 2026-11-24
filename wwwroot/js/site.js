document.addEventListener("DOMContentLoaded", () => {
  initJoypadHero();
  initJoypadButtons();
});

/* Clicking/tapping A, B, X or Y shakes the whole screen and (on phones that support it) buzzes the device. */
function initJoypadButtons() {
  const hero = document.getElementById("gpHero");
  if (!hero) return;
  const reduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  let timer;

  function shake() {
    if (!reduced) {
      document.body.classList.remove("gp-shake");
      void document.body.offsetWidth;            // restart animation on rapid clicks
      document.body.classList.add("gp-shake");
      clearTimeout(timer);
      timer = setTimeout(() => document.body.classList.remove("gp-shake"), 500);
    }
    if (navigator.vibrate) navigator.vibrate([60, 30, 60]); // Android; iOS Safari ignores this
  }

  hero.querySelectorAll(".gp-btn").forEach(btn => {
    btn.addEventListener("pointerdown", () => {
      if (!hero.classList.contains("is-built")) return;   // only once fully assembled
      btn.classList.add("is-pressed");
      shake();
    });
    const release = () => btn.classList.remove("is-pressed");
    btn.addEventListener("pointerup", release);
    btn.addEventListener("pointerleave", release);
    btn.addEventListener("pointercancel", release);
  });
}

/* Scroll-driven joypad: each SVG part has data-a="start,end" (scroll progress window 0..1)
   and data-from="x,y,rotate,scale" (where it flies in from). */
function initJoypadHero() {
  const hero = document.getElementById("gpHero");
  if (!hero) return;

  const parts = [...hero.querySelectorAll(".gp-part")].map(el => {
    const [s, e] = el.dataset.a.split(",").map(Number);
    const [x, y, r, sc] = el.dataset.from.split(",").map(Number);
    return { el, s, e, x, y, r, sc };
  });

  const clamp = (v, a = 0, b = 1) => Math.min(b, Math.max(a, v));
  const easeOut = t => 1 - Math.pow(1 - t, 3);
  const easeBack = t => { const c = 1.4; return 1 + (c + 1) * Math.pow(t - 1, 3) + c * Math.pow(t - 1, 2); };

  function render(p) {
    hero.style.setProperty("--p", p.toFixed(4));
    for (const q of parts) {
      const t = clamp((p - q.s) / (q.e - q.s));
      const k = easeOut(t);
      const b = q.el.classList.contains("gp-btn") ? easeBack(t) : k; // buttons "pop" in
      const inv = 1 - b;
      q.el.style.opacity = clamp(t * 2.5).toFixed(3);
      q.el.style.transform =
        `translate(${q.x * inv}px, ${q.y * inv}px) rotate(${q.r * inv}deg) scale(${q.sc + (1 - q.sc) * b})`;
    }
    hero.classList.toggle("is-built", p > 0.97);
  }

  const reduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
  if (reduced) { render(1); return; }

  let ticking = false;
  function update() {
    ticking = false;
    const rect = hero.getBoundingClientRect();
    const range = rect.height - window.innerHeight;
    render(clamp(-rect.top / (range * 0.85))); // finish early so the built pad holds on screen
  }
  const onScroll = () => { if (!ticking) { ticking = true; requestAnimationFrame(update); } };
  window.addEventListener("scroll", onScroll, { passive: true });
  window.addEventListener("resize", onScroll);
  update();
}
