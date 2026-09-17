const cacheName = 'smart-resume-analysis-v2';
const staticAssets = [
    '/css/site.css',
    '/js/site.js',
    '/manifest.webmanifest'
];

self.addEventListener('install', event => {
    event.waitUntil(caches.open(cacheName).then(cache => cache.addAll(staticAssets)));
    self.skipWaiting();
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys => Promise.all(keys.filter(key => key !== cacheName).map(key => caches.delete(key))))
    );
    event.waitUntil(self.clients.claim());
});

self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') {
        return;
    }

    const requestUrl = new URL(event.request.url);
    const isAsset = staticAssets.some(asset => asset === requestUrl.pathname);
    const isNavigation = event.request.mode === 'navigate';

    // Navigation requests always go to the network so the latest HTML is shown.
    if (isNavigation) {
        event.respondWith(
            fetch(event.request).catch(() => caches.match('/'))
        );
        return;
    }

    // Static assets use cache-first with a network update.
    event.respondWith(
        caches.match(event.request).then(cached => {
            const networkCopy = fetch(event.request).then(networkResponse => {
                if (networkResponse && networkResponse.ok && isAsset) {
                    const responseClone = networkResponse.clone();
                    caches.open(cacheName).then(cache => cache.put(event.request, responseClone));
                }
                return networkResponse;
            }).catch(() => cached);

            return cached || networkCopy;
        })
    );
});