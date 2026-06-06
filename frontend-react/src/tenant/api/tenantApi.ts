import api from '../../axios/axiosConf'
import type {Tenant} from '../types/tenantTypes'

export async function fetchTenants(): Promise<Tenant[]> {
  try {
    const response = await api.get('/tenants')
    return response.data
  } catch (error) {
    console.error('Error fetching tenants:', error)
    throw error
  }
}

export async function createTenant(tenant: Omit<Tenant, 'id'>): Promise<Tenant> {
  try {
    const response = await api.post('/tenants', tenant)
    return response.data
  } catch (error) {
    console.error('Error creating tenant:', error)
    throw error
  }
}