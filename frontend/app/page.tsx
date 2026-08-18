'use client';

import { useState, useEffect, useCallback } from 'react';
import { fetchPagamentos, fetchStats } from '@/lib/api';
import type { Pagamento, Stats } from '@/types';
import StatsBar from '@/components/StatsBar';
import Filters from '@/components/Filters';
import PagamentoTable from '@/components/PagamentoTable';

export default function Dashboard() {
  const [pagamentos, setPagamentos] = useState<Pagamento[]>([]);
  const [stats, setStats] = useState<Stats | null>(null);
  const [statusFilter, setStatusFilter] = useState('');
  const [contratoFilter, setContratoFilter] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [lastUpdate, setLastUpdate] = useState<Date | null>(null);
  const [autoRefresh, setAutoRefresh] = useState(true);

  const loadData = useCallback(async () => {
    setError(null);
    try {
      const [pags, st] = await Promise.all([
        fetchPagamentos(statusFilter || undefined, contratoFilter || undefined),
        fetchStats(),
      ]);
      setPagamentos(pags);
      setStats(st);
      setLastUpdate(new Date());
    } catch (err) {
      setError('Não foi possível conectar à API. Verifique se o backend está rodando.');
      console.error(err);
    } finally {
      setLoading(false);
    }
  }, [statusFilter, contratoFilter]);

  useEffect(() => {
    setLoading(true);
    loadData();
  }, [loadData]);

  useEffect(() => {
    if (!autoRefresh) return;
    const interval = setInterval(loadData, 5000);
    return () => clearInterval(interval);
  }, [loadData, autoRefresh]);

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-blue-950 text-white shadow-lg">
        <div className="max-w-7xl mx-auto px-6 py-4 flex items-center justify-between">
          <div>
            <h1 className="text-xl font-bold tracking-tight">Sabemi</h1>
            <p className="text-blue-300 text-xs mt-0.5">Painel de Monitoramento de Pagamentos</p>
          </div>
          <div className="flex items-center gap-2 text-xs text-blue-300">
            <span className="h-2 w-2 rounded-full bg-green-400 animate-pulse" />
            Sistema ativo
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-6 py-8 space-y-6">
        {/* Error Banner */}
        {error && (
          <div className="bg-red-50 border border-red-300 rounded-xl p-4 flex items-start gap-3">
            <span className="text-red-500 text-xl shrink-0">⚠</span>
            <div>
              <p className="font-semibold text-red-800">Erro de conexão</p>
              <p className="text-red-600 text-sm mt-0.5">{error}</p>
            </div>
          </div>
        )}

        {/* Stats */}
        {stats ? (
          <StatsBar stats={stats} />
        ) : (
          <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
            {[0, 1, 2, 3].map((i) => (
              <div key={i} className="rounded-xl border border-gray-200 p-5 bg-white animate-pulse h-28" />
            ))}
          </div>
        )}

        {/* Filters */}
        <Filters
          statusFilter={statusFilter}
          contratoFilter={contratoFilter}
          onStatusChange={setStatusFilter}
          onContratoChange={setContratoFilter}
          onRefresh={loadData}
          lastUpdate={lastUpdate}
          autoRefresh={autoRefresh}
          onToggleAutoRefresh={() => setAutoRefresh((v) => !v)}
        />

        {/* Table */}
        {loading ? (
          <div className="bg-white rounded-xl border border-gray-200 p-16 flex justify-center shadow-sm">
            <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-900" />
          </div>
        ) : (
          <PagamentoTable pagamentos={pagamentos} />
        )}
      </main>
    </div>
  );
}
