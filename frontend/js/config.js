// Lista de portas e URLs padrão utilizadas pelo Visual Studio e .NET CLI
const CANDIDATE_API_URLS = [
    'http://localhost:5132',
    'https://localhost:7190',
    'http://localhost:5000',
    'https://localhost:5001'
];

let API_BASE_URL = localStorage.getItem('api_base_url') || CANDIDATE_API_URLS[0];

async function fetchWithToken(url, options = {}) {
    const token = localStorage.getItem('token');
    const headers = { ...options.headers };
    if (token) headers['Authorization'] = 'Bearer ' + token;
    const finalOptions = { ...options, headers };

    if (finalOptions.body && !(finalOptions.body instanceof FormData) && typeof finalOptions.body === 'object') {
        finalOptions.headers['Content-Type'] = 'application/json';
        finalOptions.body = JSON.stringify(finalOptions.body);
    }

    try {
        const response = await fetch(url, finalOptions);
        if (response.status === 401) {
            tratarNaoAutorizado();
        }
        return response;
    } catch (networkError) {
        // Tenta fallback com as outras URLs da lista se houver falha de rede/CORS na porta atual
        const currentBase = API_BASE_URL;
        for (const candidate of CANDIDATE_API_URLS) {
            if (candidate === currentBase) continue;

            const fallbackUrl = url.includes(currentBase) 
                ? url.replace(currentBase, candidate) 
                : candidate + (url.startsWith('/') ? url : '/' + url);

            try {
                const fallbackResponse = await fetch(fallbackUrl, finalOptions);
                
                // Se a requisição respondeu com sucesso, atualiza a API_BASE_URL ativa
                API_BASE_URL = candidate;
                localStorage.setItem('api_base_url', candidate);

                if (fallbackResponse.status === 401) {
                    tratarNaoAutorizado();
                }
                return fallbackResponse;
            } catch (err) {
                // Tenta o próximo candidato
            }
        }
        throw networkError;
    }
}

function tratarNaoAutorizado() {
    localStorage.removeItem('token');
    localStorage.removeItem('usuario');
    if (window.location.pathname.includes('/html/')) {
        window.location.href = '../../index.html?unauthorized=true';
    } else {
        window.location.href = 'index.html?unauthorized=true';
    }
}
