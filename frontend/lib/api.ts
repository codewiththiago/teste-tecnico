import type { Pagamento, Stats, StatusContrato } from '@/types';

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5000';

async function apiFetch<T>(path: string): Promise<T> {
  const res = await fetch(`${API_URL}${path}`, { cache: 'no-store' });
  if (!res.ok) throw new Error(`API error ${res.status}: ${path}`);
  return res.json() as Promise<T>;
}

export function fetchPagamentos(status?: string, idContrato?: string): Promise<Pagamento[]> {
  const params = new URLSearchParams();
  if (status) params.set('status', status);
  if (idContrato) params.set('id_contrato', idContrato);
  const qs = params.toString();
  return apiFetch<Pagamento[]>(`/api/pagamentos${qs ? `?${qs}` : ''}`);
}

export function fetchStats(): Promise<Stats> {
  return apiFetch<Stats>('/api/stats');
}

export function fetchContratos(): Promise<StatusContrato[]> {
  return apiFetch<StatusContrato[]>('/api/contratos');
}
