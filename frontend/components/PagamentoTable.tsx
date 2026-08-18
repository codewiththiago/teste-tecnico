import type { Pagamento } from '@/types';

const BRL = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });

function StatusBadge({ status }: { status: string }) {
  const s = status.toLowerCase();
  const map: Record<string, string> = {
    sucesso: 'bg-green-100 text-green-800 border-green-200',
    erro: 'bg-red-100 text-red-800 border-red-200',
    pendente: 'bg-yellow-100 text-yellow-800 border-yellow-200',
  };
  const cls = map[s] ?? 'bg-gray-100 text-gray-700 border-gray-200';
  return (
    <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-semibold border ${cls}`}>
      {status}
    </span>
  );
}

function PagamentoRow({ p }: { p: Pagamento }) {
  const hasError = p.temErro || !!p.erro;

  return (
    <tr className={hasError ? 'bg-red-50 border-l-4 border-l-red-500' : 'hover:bg-gray-50 border-l-4 border-l-transparent'}>
      <td className="px-4 py-3 font-mono text-xs text-gray-700 max-w-[140px] truncate" title={p.idTransacao}>
        {p.idTransacao}
      </td>
      <td className="px-4 py-3 font-mono text-xs text-gray-700">{p.idContrato}</td>
      <td className="px-4 py-3 text-right font-semibold text-gray-900 tabular-nums">
        {BRL.format(p.valor)}
      </td>
      <td className="px-4 py-3 text-gray-600 text-sm">
        {new Date(p.dataPagamento).toLocaleDateString('pt-BR')}
      </td>
      <td className="px-4 py-3">
        <StatusBadge status={p.status} />
      </td>
      <td className="px-4 py-3 text-gray-500 text-xs whitespace-nowrap">
        {new Date(p.recebidoEm).toLocaleString('pt-BR')}
      </td>
      <td className="px-4 py-3 text-center text-lg">
        {p.processado ? (
          <span className="text-green-500" title="Processado">✓</span>
        ) : (
          <span className="text-yellow-400 animate-pulse" title="Aguardando processamento">◌</span>
        )}
      </td>
      <td className="px-4 py-3 max-w-[220px]">
        {hasError && (
          <div className="flex items-start gap-1.5 bg-red-100 border border-red-200 rounded-lg px-2 py-1.5">
            <span className="text-red-500 text-sm shrink-0 mt-0.5">⚠</span>
            <span className="text-red-700 text-xs leading-snug break-words">{p.erro}</span>
          </div>
        )}
      </td>
    </tr>
  );
}

export default function PagamentoTable({ pagamentos }: { pagamentos: Pagamento[] }) {
  if (pagamentos.length === 0) {
    return (
      <div className="bg-white rounded-xl border border-gray-200 p-16 text-center shadow-sm">
        <p className="text-4xl mb-3">📭</p>
        <p className="text-gray-500 font-medium">Nenhum pagamento encontrado</p>
        <p className="text-gray-400 text-sm mt-1">Os dados aparecerão aqui quando o banco enviar notificações</p>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 border-b border-gray-200">
            <tr>
              <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">ID Transação</th>
              <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">ID Contrato</th>
              <th className="text-right px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Valor</th>
              <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Data Pag.</th>
              <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Status</th>
              <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Recebido Em</th>
              <th className="text-center px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Proc.</th>
              <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wide">Erro</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {pagamentos.map((p) => (
              <PagamentoRow key={p.id} p={p} />
            ))}
          </tbody>
        </table>
      </div>
      <div className="px-4 py-2 bg-gray-50 border-t border-gray-100 text-xs text-gray-400">
        {pagamentos.length} registro{pagamentos.length !== 1 ? 's' : ''} encontrado{pagamentos.length !== 1 ? 's' : ''}
      </div>
    </div>
  );
}
