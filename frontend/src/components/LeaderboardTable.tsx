import React from 'react';
import { 
  TableContainer, Paper, Table, TableHead, TableRow, 
  TableCell, TableBody, Stack, Avatar, Typography, Chip, LinearProgress 
} from '@mui/material';
import { Performer } from './types';

interface LeaderboardTableProps {
  performers: Performer[];
}

export default function LeaderboardTable({ performers }: LeaderboardTableProps) {
  if (performers.length === 0) return null;

  const formatValue = (val: number) => new Intl.NumberFormat('cs-CZ').format(val);

  return (
    <TableContainer component={Paper} variant="outlined" sx={{ borderRadius: 2, overflowX: 'auto' }}>
      <Table sx={{ minWidth: 650 }}>
        <TableHead sx={{ bgcolor: '#f9f9f9' }}>
          <TableRow>
            <TableCell sx={{ width: 50, fontWeight: 'bold', color: 'text.secondary' }}>#</TableCell>
            <TableCell sx={{ fontWeight: 'bold', color: 'text.secondary' }}>OBCHODNÍK</TableCell>
            <TableCell align="right" sx={{ fontWeight: 'bold', color: 'text.secondary' }}>DEALY</TableCell>
            <TableCell align="right" sx={{ fontWeight: 'bold', color: 'text.secondary' }}>HODNOTA</TableCell>
            <TableCell align="right" sx={{ fontWeight: 'bold', color: 'text.secondary' }}>TREND</TableCell>
            <TableCell align="right" sx={{ fontWeight: 'bold', color: 'text.secondary' }}>PODÍL</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {performers.map((row) => (
            <TableRow key={row.rank} hover sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
              <TableCell sx={{ color: 'text.secondary' }}>{row.rank}</TableCell>
              <TableCell>
                <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
                  <Avatar src={row.avatarUrl || ''} sx={{ width: 32, height: 32 }}>
                    {!row.avatarUrl && row.name.charAt(0)}
                  </Avatar>
                  <Typography variant="body2" sx={{ fontWeight: 'medium' }}>{row.name}</Typography>
                </Stack>
              </TableCell>
              <TableCell align="right">
                <Typography variant="body2" sx={{ fontWeight: 'bold', display: 'inline' }}>{row.deals}</Typography>
                <Typography variant="caption" color="textSecondary" sx={{ ml: 0.5 }}>dealů</Typography>
              </TableCell>
              <TableCell align="right">
                <Typography variant="body2" sx={{ fontWeight: 'bold', display: 'inline', whiteSpace: 'nowrap' }}>{formatValue(row.value)}</Typography>
                <Typography variant="caption" color="textSecondary" sx={{ ml: 0.5 }}>Kč</Typography>
              </TableCell>
              <TableCell align="right">
                <Chip 
                  size="small" 
                  label={`${row.trend > 0 ? '+' : ''}${row.trend} %`} 
                  color={row.trend > 0 ? 'success' : 'error'}
                  variant="outlined"
                  sx={{ bgcolor: row.trend > 0 ? '#e8f5e9' : '#ffebee', fontWeight: 'bold', border: 'none' }}
                />
              </TableCell>
              <TableCell align="right">
                <Stack direction="row" spacing={1} sx={{ alignItems: 'center', justifyContent: 'flex-end' }}>
                  <LinearProgress variant="determinate" value={row.share} color="info" sx={{ width: 60, height: 6, borderRadius: 3 }} />
                  <Typography variant="caption" color="textSecondary" sx={{ width: 35 }}>{row.share} %</Typography>
                </Stack>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}