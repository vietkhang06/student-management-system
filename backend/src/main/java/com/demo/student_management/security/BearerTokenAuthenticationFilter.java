package com.demo.student_management.security;

import com.demo.student_management.entity.TaiKhoan;
import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import java.io.IOException;
import java.util.List;

@Component
@RequiredArgsConstructor
public class BearerTokenAuthenticationFilter extends OncePerRequestFilter {

    private static final String BEARER_PREFIX = "Bearer ";

    private final AuthTokenService authTokenService;

    @Override
    protected void doFilterInternal(
            HttpServletRequest request,
            HttpServletResponse response,
            FilterChain filterChain
    ) throws ServletException, IOException {

        String authorization = request.getHeader("Authorization");
        if (authorization != null && authorization.startsWith(BEARER_PREFIX)) {
            String token = authorization.substring(BEARER_PREFIX.length());
            authTokenService.resolveToken(token).ifPresent(this::authenticate);
        }

        filterChain.doFilter(request, response);
    }

    private void authenticate(TaiKhoan taiKhoan) {
        AuthenticatedUser principal = new AuthenticatedUser(
                taiKhoan.getIdTaiKhoan(),
                taiKhoan.getTenDangNhap(),
                taiKhoan.getLoaiTaiKhoan()
        );

        var authentication = new UsernamePasswordAuthenticationToken(
                principal,
                null,
                List.of(new SimpleGrantedAuthority("ROLE_" + taiKhoan.getLoaiTaiKhoan()))
        );

        SecurityContextHolder.getContext().setAuthentication(authentication);
    }
}
