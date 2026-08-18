import type { Stats } from '@/types';

interface CardProps {
  label: string;
  value: number;
  colorClass: string;
  icon: string;
}

function StatCard({ label, value, colorClass, icon }: CardProps) {
  return (
    <div className={`rounded-xl border p-5 ${colorClass}`}>
      <div className="flex items-center justify-between">
        <p className="text-sm font-medium opacity-70">{label}</p>
        <span className="text-2xl">{icon}</span>
      </div>
      <p className="text-4xl font-bold mt-2 tabular-nums">{value}</p>
    </div>
  );
}

export default function StatsBar({ stats }: { stats: Stats }) {
  return (
    <div className="grid grid-cols-2 lg:grid-cols-4 gap-4">
      <StatCard
        label="Total de Eventos"
        value={stats.total}
        colorClass="bg-blue-50 border-blue-200 text-blue-800"
        icon="📨"
      />
      <StatCard
        label="Processados"
        value={stats.processados}
        colorClass="bg-green-50 border-green-200 text-green-800"
        icon="✅"
      />
      <StatCard
        label="Com Erro"
        value={stats.comErro}
        colorClass="bg-red-50 border-red-200 text-red-800"
        icon="⚠️"
      />
      <StatCard
        label="Pendentes"
        value={stats.pendentes}
        colorClass="bg-yellow-50 border-yellow-200 text-yellow-800"
        icon="⏳"
      />
    </div>
  );
}
