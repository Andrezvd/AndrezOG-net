import { useState, type ChangeEvent, type FormEvent } from 'react'

import { setAuthToken } from '../../axios/axiosConf'
import { registerUser } from '../api/authApi'
import type { RegisterFormState } from '../types/authTypes'

const initialForm: RegisterFormState = {
	email: '',
	password: '',
	confirmPassword: '',
	name: '',
	lastName: '',
	phoneNumber: '',
	country: '',
}

export default function Register() {
	const [form, setForm] = useState<RegisterFormState>(initialForm)
	const [loading, setLoading] = useState(false)
	const [error, setError] = useState('')
	const [success, setSuccess] = useState('')

	function handleChange(event: ChangeEvent<HTMLInputElement>) {
		const { name, value } = event.target
		setForm((current) => ({ ...current, [name]: value }))
	}

	async function handleSubmit(event: FormEvent<HTMLFormElement>) {
		event.preventDefault()
		setError('')
		setSuccess('')
		setLoading(true)

		try {
			const response = await registerUser(form)
			setAuthToken(response.token)
			setSuccess(`${response.message} (${response.email} - ${response.role})`)
			setForm(initialForm)
		} catch (err) {
			setError(err instanceof Error ? err.message : 'Error al registrar')
		} finally {
			setLoading(false)
		}
	}

	return (
		<section className="register-section">
			<h1>Registro</h1>
			<p>Solo este formulario consume el endpoint <code>/auth/register</code>.</p>

			<form className="register-form" onSubmit={handleSubmit}>
				<input name="email" placeholder="Email" value={form.email} onChange={handleChange} />
				<input name="password" placeholder="Password" type="password" value={form.password} onChange={handleChange} />
				<input name="confirmPassword" placeholder="Confirm password" type="password" value={form.confirmPassword} onChange={handleChange} />
				<input name="name" placeholder="Name" value={form.name} onChange={handleChange} />
				<input name="lastName" placeholder="Last name" value={form.lastName} onChange={handleChange} />
				<input name="phoneNumber" placeholder="Phone number" value={form.phoneNumber} onChange={handleChange} />
				<input name="country" placeholder="Country" value={form.country} onChange={handleChange} />

				<button type="submit" disabled={loading}>
					{loading ? 'Registrando...' : 'Registrarse'}
				</button>
			</form>

			{error ? <p style={{ color: 'crimson' }}>{error}</p> : null}
			{success ? <p style={{ color: 'green' }}>{success}</p> : null}
		</section>
	)
}
