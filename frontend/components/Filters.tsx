'use client';

interface FiltersProps {
  statusFilter: string;
  contratoFilter: string;
  onStatusChange: (v: string) => void;
  onContratoChange: (v: string) => void;
  onRefresh: () => void;
  lastUpdate: Date | null;
  autoRefresh: boolean;
  onToggleAutoRefresh: () => void;
}

export default function Filters({
  statusFilter,
  contratoFilter,
  onStatusChange,
  onContratoChange,
  onRefresh,
  lastUpdate,
  autoRefresh,
  onToggleAutoRefresh,
}: FiltersProps) {
  return (
    <div className="bg-white rounded-xl border border-gray-200 p-4 shadow-sm">
      <div className="flex flex-wrap items-end gap-4">
        <div>
          <label className="block text-xs font-semibold text-gray-500 uppercase tracking-wide mb-1">
            Status
          </label>
          <select
            value={statusFilter}
            onChange={(e) => onStatusChange(e.target.value)}
            className="rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 bg-white"
          >
            <option value="">Todos</option>
            <option value="sucesso">Sucesso</option>
            <option value="erro">Erro</option>
            <option value="pendente">Pendente</option>
          </select>
        </div>

        <div>
          <label className="block text-xs font-semibold text-gray-500 uppercase tracking-wide mb-1">
            ID do Contrato
          </label>
          <input
            type="text"
            value={contratoFilter}
            onChange={(e) => onContratoChange(e.target.value)}
            placeholder="Buscar por contrato..."
            className="rounded-lg border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 w-56"
          />
        </div>

        <div className="ml-auto flex items-center gap-3">
          <button
            onClick={onToggleAutoRefresh}
            className={`flex items-center gap-1.5 px-3 py-2 rounded-lg text-sm border transition-colors ${
              autoRefresh
                ? 'bg-green-50 border-green-300 text-green-700'
                : 'bg-gray-50 border-gray-300 text-gray-600'
            }`}
          >
            <span className={autoRefresh ? 'animate-pulse' : ''}>●</span>
            {autoRefresh ? 'Auto (5s)' : 'Auto off'}
          </button>

          {lastUpdate && (
            <span className="text-xs text-gray-400 hidden sm:block">
              Atualizado às {lastUpdate.toLocaleTimeString('pt-BR')}
            </span>
          )}

          <button
            onClick={onRefresh}
            className="px-4 py-2 bg-blue-900 text-white rounded-lg text-sm hover:bg-blue-800 transition-colors font-medium"
          >
            Atualizar
          </button>
        </div>
      </div>
    </div>
  );
}
