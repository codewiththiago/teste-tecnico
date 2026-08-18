export interface Pagamento {
  id: string;
  idTransacao: string;
  idContrato: string;
  valor: number;
  dataPagamento: string;
  status: string;
  recebidoEm: string;
  processado: boolean;
  erro: string | null;
  processadoEm: string | null;
  temErro: boolean;
}

export interface Stats {
  total: number;
  comErro: number;
  processados: number;
  pendentes: number;
}

export interface StatusContrato {
  id: string;
  idContrato: string;
  ultimoPagamentoId: string;
  valorTotal: number;
  status: string;
  ultimaAtualizacao: string;
  totalPagamentos: number;
}
